using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public class TimeWarpController : MonoBehaviour
{
    [Header("相机设置")]
    public Camera mainCamera;
    public Camera secondaryCamera;
    public float cameraCrossFadeDuration = 1.0f; // 相机淡入淡出时间
    
    [Header("时钟设置")]
    public RectTransform closedClockUI;    // 左上角关闭的时钟UI
    public RectTransform openClockUI;      // 打开的时钟UI
    public float clockAnimationDuration = 1.5f;  // 时钟动画持续时间
    public float bounceAmplitude = 20f;    // 弹跳振幅
    public float bounceFrequency = 3f;     // 弹跳频率
    public float bounceDecay = 5f;         // 弹跳衰减速度
    
    [Header("玩家设置")]
    public Transform playerTransform;     // 玩家角色的Transform
    public float playerYOffset = 20f;     // 玩家Y轴移动距离
    
    [Header("效果设置")]
    public float transitionDuration = 2.0f;     // 过渡效果持续时间
    public Volume postProcessingVolume;         // URP Volume组件
    
    [Header("淡入淡出设置")]
    public Image fadeImage;               // 用于淡入淡出的黑色图像
    public float fadeSpeed = 2.0f;        // 淡入淡出速度

    [Header("暂停控制")] 
    public PauseUI pause;
    
    public Canvas canvas;
    
    private bool isWarping = false;
    private bool isInWarpedState = false; // 是否处于穿越后的状态
    
    private Vector2 closedClockOriginalPosition;  // 关闭时钟的原始位置
    private Vector2 openClockCenterPosition;      // 打开时钟的中心位置
    private Vector2 openClockOffscreenPosition;   // 打开时钟的屏幕外位置
    
    private ChromaticAberration chromaticAberration;   // 色差效果
    private LensDistortion lensDistortion;            // 镜头扭曲效果
    private DepthOfField depthOfField;                // 景深效果用于模糊
    private Vignette vignette;                        // 渐晕效果增强时空感
    
    private void Awake()
    {
        // 确保找到URP Volume效果
        if (postProcessingVolume != null && postProcessingVolume.profile != null)
        {
            postProcessingVolume.profile.TryGet(out chromaticAberration);
            postProcessingVolume.profile.TryGet(out lensDistortion);
            postProcessingVolume.profile.TryGet(out depthOfField);
            postProcessingVolume.profile.TryGet(out vignette);
        }
        
        // 初始化时禁用二级相机
        if (secondaryCamera != null)
        {
            secondaryCamera.gameObject.SetActive(false);
        }
        
        // 初始化淡入淡出图像
        if (fadeImage != null)
        {
            // 确保淡入淡出图像初始时是透明的
            Color fadeColor = fadeImage.color;
            fadeColor.a = 0f;
            fadeImage.color = fadeColor;
        }
        
        // 初始化时钟位置
        InitializeClockPositions();
    }
    
    private void InitializeClockPositions()
    {
        if (closedClockUI != null)
        {
            // 保存关闭时钟的原始位置
            closedClockOriginalPosition = closedClockUI.anchoredPosition;
        }
        
        if (openClockUI != null)
        {
            // 获取UI画布
            canvas = openClockUI.GetComponentInParent<Canvas>();
            if (canvas != null)
            {
                RectTransform canvasRect = canvas.GetComponent<RectTransform>();
                
                // 计算画布中心位置
                openClockCenterPosition = Vector2.zero;
                
                // 如果时钟使用的不是中心锚点，需要进行调整
                if (openClockUI.anchorMin != new Vector2(0.5f, 0.5f) || openClockUI.anchorMax != new Vector2(0.5f, 0.5f))
                {
                    Vector2 anchorCenter = (openClockUI.anchorMin + openClockUI.anchorMax) * 0.5f;
                    Vector2 canvasSize = canvasRect.sizeDelta;
                    openClockCenterPosition = new Vector2(
                        (0.5f - anchorCenter.x) * canvasSize.x,
                        (0.5f - anchorCenter.y) * canvasSize.y
                    );
                }
                
                // 计算屏幕下方的位置（屏幕外）
                float offscreenY = -canvasRect.sizeDelta.y * 0.5f - openClockUI.sizeDelta.y;
                openClockOffscreenPosition = new Vector2(openClockCenterPosition.x, offscreenY);
                
                // 初始时将打开的时钟放在屏幕外
                openClockUI.anchoredPosition = openClockOffscreenPosition;
                openClockUI.gameObject.SetActive(false);
                
                Debug.Log($"Closed clock position: {closedClockOriginalPosition}");
                Debug.Log($"Open clock center position: {openClockCenterPosition}");
                Debug.Log($"Open clock offscreen position: {openClockOffscreenPosition}");
            }
            else
            {
                Debug.LogError("Cannot find Canvas component in parents of openClockUI!");
            }
        }
    }
    
    // 外部调用此方法开始时空穿越
    public void StartTimeWarp()
    {
        if (pause.GetPauseStatus())
            return;
        if (!isWarping)
        {
            isWarping = true;
            
            if (!isInWarpedState)
            {
                // 穿越到未来
                StartCoroutine(TimeWarpForwardSequence());
            }
            else
            {
                // 穿越回过去
                StartCoroutine(TimeWarpBackwardSequence());
            }
        }
    }
    
    // 穿越到未来的协程序列
    private IEnumerator TimeWarpForwardSequence()
    {
        pause.OnExternPause();
        // 1. 启用并动画过渡后处理效果
        StartCoroutine(AnimatePostProcessing(true));
        
        // 2. 时钟动画：关闭时钟移出，打开时钟弹入
        yield return StartCoroutine(AnimateClockStartSequence());
        
        // 3. 淡出屏幕
        yield return StartCoroutine(FadeScreen(true));
        
        // 4. 移动玩家并切换相机
        MovePlayerAndSwitchCamera(true);
        
        // 5. 淡入屏幕
        yield return StartCoroutine(FadeScreen(false));
        
        // 6. 时钟动画：打开时钟移出，关闭时钟移入
        yield return StartCoroutine(AnimateClockEndSequence());
        
        // 7. 禁用后处理效果
        StartCoroutine(AnimatePostProcessing(false));
        pause.OnExternResume();
        // 8. 更新状态
        isInWarpedState = true;
        isWarping = false;
    }
    
    // 穿越回过去的协程序列
    private IEnumerator TimeWarpBackwardSequence()
    {
        pause.OnExternPause();
        // 1. 启用并动画过渡后处理效果
        StartCoroutine(AnimatePostProcessing(true));
        
        // 2. 时钟动画：关闭时钟移出，打开时钟弹入
        yield return StartCoroutine(AnimateClockStartSequence());
        
        // 3. 淡出屏幕
        yield return StartCoroutine(FadeScreen(true));
        
        // 4. 移动玩家并切换相机
        MovePlayerAndSwitchCamera(false);
        
        // 5. 淡入屏幕
        yield return StartCoroutine(FadeScreen(false));
        
        // 6. 时钟动画：打开时钟移出，关闭时钟移入
        yield return StartCoroutine(AnimateClockEndSequence());
        
        // 7. 禁用后处理效果
        StartCoroutine(AnimatePostProcessing(false));
        
        // 8. 更新状态
        pause.OnExternResume();
        isInWarpedState = false;
        isWarping = false;
    }
    
    // 时空穿越开始时的时钟动画（关闭时钟移出，打开时钟弹入）
    private IEnumerator AnimateClockStartSequence()
    {
        // 1. 关闭时钟快速移出屏幕
        if (closedClockUI != null)
        {
            Vector2 offscreenPosition = new Vector2(
                closedClockOriginalPosition.x - closedClockUI.sizeDelta.x * 2,
                closedClockOriginalPosition.y
            );
            
            yield return StartCoroutine(AnimateUIElement(
                closedClockUI, 
                closedClockOriginalPosition, 
                offscreenPosition, 
                clockAnimationDuration * 0.5f, 
                EaseType.EaseOutQuad
            ));
            
            closedClockUI.gameObject.SetActive(false);
        }
        
        // 2. 打开时钟从下方弹入屏幕中央
        if (openClockUI != null)
        {
            openClockUI.gameObject.SetActive(true);
            openClockUI.anchoredPosition = openClockOffscreenPosition;
            
            yield return StartCoroutine(AnimateUIElementWithBounce(
                openClockUI,
                openClockOffscreenPosition,
                openClockCenterPosition,
                clockAnimationDuration,
                bounceAmplitude,
                bounceFrequency,
                bounceDecay
            ));
        }
    }
    
    // 时空穿越结束时的时钟动画（打开时钟移出，关闭时钟移入）
    private IEnumerator AnimateClockEndSequence()
    {
        // 1. 打开时钟快速向下移出屏幕
        if (openClockUI != null)
        {
            yield return StartCoroutine(AnimateUIElement(
                openClockUI,
                openClockCenterPosition,
                openClockOffscreenPosition,
                clockAnimationDuration * 0.5f,
                EaseType.EaseInQuad
            ));
            
            openClockUI.gameObject.SetActive(false);
        }
        
        // 2. 关闭时钟从左上角移回原位
        if (closedClockUI != null)
        {
            Vector2 offscreenPosition = new Vector2(
                closedClockOriginalPosition.x - closedClockUI.sizeDelta.x * 2,
                closedClockOriginalPosition.y
            );
            
            closedClockUI.gameObject.SetActive(true);
            closedClockUI.anchoredPosition = offscreenPosition;
            
            yield return StartCoroutine(AnimateUIElement(
                closedClockUI,
                offscreenPosition,
                closedClockOriginalPosition,
                clockAnimationDuration * 0.5f,
                EaseType.EaseOutQuad
            ));
        }
    }
    
    // 带弹跳效果的UI元素动画
    private IEnumerator AnimateUIElementWithBounce(
        RectTransform uiElement, 
        Vector2 startPosition, 
        Vector2 targetPosition, 
        float duration,
        float amplitude,
        float frequency,
        float decay)
    {
        float elapsed = 0f;
        
        while (elapsed < duration)
        {
            float t = elapsed / duration;
            
            // 使用缓入缓出函数使移动更自然
            float smoothT = EaseInOutQuad(t);
            
            // 基础移动
            Vector2 basePosition = Vector2.Lerp(startPosition, targetPosition, smoothT);
            
            // 添加弹跳效果（只在Y轴上）
            // 弹跳效果在接近目标时更明显
            float bouncePhase = t * frequency * Mathf.PI;
            float bounceT = Mathf.Clamp01((t - 0.5f) * 2); // 在后半段开始弹跳
            float bounceAmount = amplitude * bounceT * Mathf.Exp(-decay * bounceT) * Mathf.Sin(bouncePhase);
            
            // 应用位置
            Vector2 finalPosition = basePosition + new Vector2(0, bounceAmount);
            uiElement.anchoredPosition = finalPosition;
            
            elapsed += Time.deltaTime;
            yield return null;
        }
        
        // 确保最终位置精确
        uiElement.anchoredPosition = targetPosition;
    }
    
    // 基础UI元素动画
    private IEnumerator AnimateUIElement(
        RectTransform uiElement, 
        Vector2 startPosition, 
        Vector2 targetPosition, 
        float duration,
        EaseType easeType)
    {
        float elapsed = 0f;
        
        while (elapsed < duration)
        {
            float t = elapsed / duration;
            float easedT;
            
            // 应用缓动函数
            switch (easeType)
            {
                case EaseType.EaseInQuad:
                    easedT = t * t;
                    break;
                case EaseType.EaseOutQuad:
                    easedT = t * (2 - t);
                    break;
                case EaseType.EaseInOutQuad:
                    easedT = t < 0.5f ? 2 * t * t : -1 + (4 - 2 * t) * t;
                    break;
                default:
                    easedT = t;
                    break;
            }
            
            uiElement.anchoredPosition = Vector2.Lerp(startPosition, targetPosition, easedT);
            
            elapsed += Time.deltaTime;
            yield return null;
        }
        
        // 确保最终位置精确
        uiElement.anchoredPosition = targetPosition;
    }
    
    // 缓动函数
    private float EaseInOutQuad(float t)
    {
        return t < 0.5f ? 2 * t * t : -1 + (4 - 2 * t) * t;
    }
    
    // 屏幕淡入淡出效果
    private IEnumerator FadeScreen(bool fadeOut)
    {
        if (fadeImage == null) yield break;
        
        float targetAlpha = fadeOut ? 1f : 0f;
        Color currentColor = fadeImage.color;
        float startAlpha = currentColor.a;
        float elapsed = 0f;
        
        while (elapsed < cameraCrossFadeDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / cameraCrossFadeDuration);
            float smoothT = Mathf.SmoothStep(0, 1, t);
            
            currentColor.a = Mathf.Lerp(startAlpha, targetAlpha, smoothT);
            fadeImage.color = currentColor;
            
            yield return null;
        }
        
        // 确保最终值精确
        currentColor.a = targetAlpha;
        fadeImage.color = currentColor;
    }
    
    // URP后处理效果动画过渡
    private IEnumerator AnimatePostProcessing(bool fadeIn)
    {
        if (postProcessingVolume == null) yield break;
        
        float startWeight = fadeIn ? 0f : 1f;
        float targetWeight = fadeIn ? 1f : 0f;
        float elapsed = 0f;
        
        // 初始权重
        postProcessingVolume.weight = startWeight;
        
        // 确保效果被启用
        SetupPostProcessingEffects(true);
        
        while (elapsed < transitionDuration / 2f) // 使特效过渡比时钟动画快一些
        {
            float t = elapsed / (transitionDuration / 2f);
            float smoothT = Mathf.SmoothStep(0, 1, t);
            
            // 更新Volume权重
            postProcessingVolume.weight = Mathf.Lerp(startWeight, targetWeight, smoothT);
            
            // 动态调整个别效果的强度
            if (lensDistortion != null && lensDistortion.active)
            {
                lensDistortion.intensity.value = fadeIn ? 
                    Mathf.Lerp(0f, 0.5f, smoothT) : 
                    Mathf.Lerp(0.5f, 0f, smoothT);
            }
            
            if (chromaticAberration != null && chromaticAberration.active)
            {
                chromaticAberration.intensity.value = fadeIn ? 
                    Mathf.Lerp(0f, 1f, smoothT) : 
                    Mathf.Lerp(1f, 0f, smoothT);
            }
            
            elapsed += Time.deltaTime;
            yield return null;
        }
        
        // 确保最终状态
        postProcessingVolume.weight = targetWeight;
        
        // 如果完全淡出，则禁用效果
        if (!fadeIn)
        {
            SetupPostProcessingEffects(false);
        }
    }
    
    // 设置URP后处理效果
    private void SetupPostProcessingEffects(bool enable)
    {
        if (lensDistortion != null)
        {
            lensDistortion.active = enable;
            if (enable)
            {
                lensDistortion.intensity.Override(0f); // 初始值，会通过动画过渡
                lensDistortion.scale.Override(0.8f);
            }
        }
        
        if (chromaticAberration != null)
        {
            chromaticAberration.active = enable;
            if (enable)
            {
                chromaticAberration.intensity.Override(0f); // 初始值，会通过动画过渡
            }
        }
        
        if (depthOfField != null)
        {
            depthOfField.active = enable;
            if (enable)
            {
                depthOfField.mode.Override(DepthOfFieldMode.Bokeh);
                depthOfField.focusDistance.Override(10f);
                depthOfField.aperture.Override(5.6f);
                depthOfField.focalLength.Override(50f);
            }
        }
        
        if (vignette != null)
        {
            vignette.active = enable;
            if (enable)
            {
                vignette.intensity.Override(0.4f);
                vignette.color.Override(Color.blue); // 蓝色调增强时空感
            }
        }
    }
    
    // 移动玩家并切换相机
    private void MovePlayerAndSwitchCamera(bool forward)
    {
        // 移动玩家
        if (playerTransform != null)
        {
            Vector3 newPosition = playerTransform.position;
            
            if (forward)
            {
                // 向未来穿越，玩家下移
                newPosition.y -= playerYOffset;
            }
            else
            {
                // 向过去穿越，玩家上移
                newPosition.y += playerYOffset;
            }
            
            playerTransform.position = newPosition;
        }
        
        // 切换相机
        if (mainCamera != null && secondaryCamera != null)
        {
            if (forward)
            {
                // 向未来穿越，切换到次级相机
                mainCamera.gameObject.SetActive(false);
                secondaryCamera.gameObject.SetActive(true);
            }
            else
            {
                // 向过去穿越，切换回主相机
                secondaryCamera.gameObject.SetActive(false);
                mainCamera.gameObject.SetActive(true);
            }
        }
    }
    
    // 重置所有效果（可选，用于测试）
    public void ResetEffects()
    {
        if (mainCamera != null && secondaryCamera != null)
        {
            mainCamera.gameObject.SetActive(true);
            secondaryCamera.gameObject.SetActive(false);
        }
        
        if (postProcessingVolume != null)
        {
            postProcessingVolume.weight = 0f;
            SetupPostProcessingEffects(false);
        }
        
        if (closedClockUI != null)
        {
            closedClockUI.gameObject.SetActive(true);
            closedClockUI.anchoredPosition = closedClockOriginalPosition;
        }
        
        if (openClockUI != null)
        {
            openClockUI.gameObject.SetActive(false);
            openClockUI.anchoredPosition = openClockOffscreenPosition;
        }
        
        if (fadeImage != null)
        {
            Color color = fadeImage.color;
            color.a = 0f;
            fadeImage.color = color;
        }
        
        isWarping = false;
        isInWarpedState = false;
    }
    
    // 缓动类型枚举
    private enum EaseType
    {
        Linear,
        EaseInQuad,
        EaseOutQuad,
        EaseInOutQuad
    }
}
