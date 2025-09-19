using UnityEngine;

public class ShadowVisualEffect : MonoBehaviour
{
    [SerializeField] private float fadeDistance = 0.9f;
    [SerializeField] private float fadeEdgeSize = 0.1f;
    
    private SpriteRenderer spriteRenderer;
    private Material shadowMaterial;
    
    private static readonly int FadeStartID = Shader.PropertyToID("_FadeStart");
    private static readonly int FadeEndID = Shader.PropertyToID("_FadeEnd");
    
    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null && spriteRenderer.material != null)
        {
            // Create a unique instance of the material
            shadowMaterial = new Material(spriteRenderer.material);
            shadowMaterial.SetFloat(FadeStartID, fadeDistance);
            shadowMaterial.SetFloat(FadeEndID, fadeDistance + fadeEdgeSize);
            spriteRenderer.material = shadowMaterial;
        }
    }
}