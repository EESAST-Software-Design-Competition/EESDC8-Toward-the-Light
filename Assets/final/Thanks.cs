using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Renderer))]
public class Thanks : MonoBehaviour
{
    [Tooltip("淡入持续时间（秒）")]
    public float fadeDuration = 2.0f;
    
    public float delay = 127f; // 2分7秒 = 127秒
    private Renderer targetRenderer;
    private Material material;
    private Color originalColor;

    void Start()
    {
        targetRenderer = GetComponent<Renderer>();
        material = targetRenderer.material;
        originalColor = material.color;

        // 初始设置为完全透明
        SetAlpha(0f);
        
        StartCoroutine(FadeInAfterDelay());
    }

    IEnumerator FadeInAfterDelay()
    {
        // 等待指定时间
        yield return new WaitForSeconds(delay);

        // 渐变过程
        float timer = 0f;
        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            float alpha = Mathf.Lerp(0f, 1f, timer / fadeDuration);
            SetAlpha(alpha);
            yield return null;
        }

        // 确保最终完全不透明
        SetAlpha(1f);
    }

    void SetAlpha(float alpha)
    {
        Color newColor = originalColor;
        newColor.a = alpha;
        material.color = newColor;
    }

    void OnDestroy()
    {
        // 清理创建的材质实例
        if (material != null)
            Destroy(material);
    }
}