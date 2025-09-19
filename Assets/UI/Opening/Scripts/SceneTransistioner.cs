using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneTransitioner : MonoBehaviour
{
    [Header("淡入淡出设置")]
    [SerializeField] private float fadeTime = 0.5f;       // 淡入淡出时间
    public string targetSceneName = "Scene1"; // 目标场景名称
    [SerializeField] private KeyCode triggerKey = KeyCode.Mouse0; // 触发键(鼠标左键)
    private Image fadePanel;    // 用于淡入淡出的黑色面板
    private GameObject canvasObj;
    public GameObject playerObject;
    private void Awake()
    {
        // 创建UI画布和淡入淡出面板
        
        // 确保这个脚本不会在场景切换时被销毁
    }

    private void CreateFadePanel()
    {
        if (canvasObj != null)
            return;
        // 创建画布
        canvasObj = new GameObject("TransitionCanvas");
        Canvas canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 999; // 确保在最上层
        
        // 添加CanvasScaler组件以适应不同分辨率
        CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        
        // 添加GraphicRaycaster组件
        canvasObj.AddComponent<GraphicRaycaster>();
        
        // 创建黑色面板
        GameObject panelObj = new GameObject("FadePanel");
        panelObj.transform.SetParent(canvasObj.transform, false);
        
        fadePanel = panelObj.AddComponent<Image>();
        fadePanel.color = new Color(0, 0, 0, 0); // 初始设为透明
        
        // 设置面板大小为全屏
        RectTransform rectTransform = panelObj.GetComponent<RectTransform>();
        rectTransform.anchorMin = Vector2.zero;
        rectTransform.anchorMax = Vector2.one;
        rectTransform.sizeDelta = Vector2.zero;
        
        // 将画布添加到当前游戏对象
        canvasObj.transform.SetParent(transform);
    }

    private void DestroyCanvasObj()
    {
        if (canvasObj != null)
        {
            Destroy(canvasObj);
        }
    }
    public void StartSceneTransition()
    {
        StartCoroutine(TransitionToScene());
    }
    private IEnumerator TransitionToScene()
    {
        CreateFadePanel();
        playerObject = GameObject.FindWithTag("Player");
        if (playerObject != null)
            playerObject.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;
        // 淡入（屏幕变黑）
        yield return StartCoroutine(FadeIn());
        
        // 加载新场景
        SceneManager.LoadScene(targetSceneName);
        
        // 等待一帧确保场景已加载
        yield return null;
        if (playerObject != null)
            playerObject.transform.position = GameObject.FindWithTag("Respawn").transform.position;
        // 淡出（屏幕变亮）
        yield return StartCoroutine(FadeOut());
        if (playerObject != null)
            playerObject.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
        DestroyCanvasObj();
    }

    private IEnumerator FadeIn()
    {
        float elapsedTime = 0;
        
        // 逐渐将alpha从0变为1
        while (elapsedTime < fadeTime)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Clamp01(elapsedTime / fadeTime);
            fadePanel.color = new Color(0, 0, 0, alpha);
            yield return null;
        }
        
        // 确保最终alpha为1
        fadePanel.color = new Color(0, 0, 0, 1);
    }

    private IEnumerator FadeOut()
    {
        float elapsedTime = 0;
        
        // 逐渐将alpha从1变为0
        while (elapsedTime < fadeTime)
        {
            elapsedTime += Time.deltaTime;
            float alpha = 1 - Mathf.Clamp01(elapsedTime / fadeTime);
            fadePanel.color = new Color(0, 0, 0, alpha);
            yield return null;
        }
        
        // 确保最终alpha为0
        fadePanel.color = new Color(0, 0, 0, 0);
    }
}