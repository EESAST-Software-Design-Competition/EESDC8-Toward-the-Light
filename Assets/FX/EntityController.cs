using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class EntityController : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private Material entityMaterial;
    
    [SerializeField] private LightRangeHandler lightRangeHandler;
    [SerializeField] private float defaultAlpha = 0.4f;
    [SerializeField] private float visibilityThreshold = 0.25f;
    
    // 着色器相关属性
    private static readonly int LightPositionID = Shader.PropertyToID("_LightPosition");
    private static readonly int LightRadiusID = Shader.PropertyToID("_LightRadius");
    private static readonly int LightIntensityID = Shader.PropertyToID("_LightIntensity");
    private static readonly int LightEnabledID = Shader.PropertyToID("_LightEnabled");
    private static readonly int DefaultAlphaID = Shader.PropertyToID("_DefaultAlpha");
    private static readonly int MaxLightIntensityID = Shader.PropertyToID("_MaxLightIntensity");
    private static readonly int VisibilityThresholdID = Shader.PropertyToID("_VisibilityThreshold");
    
    void Start()
    {
        // 获取组件
        spriteRenderer = GetComponent<SpriteRenderer>();
        
        // 创建材质实例，避免影响其他使用相同材质的实体
        entityMaterial = new Material(Shader.Find("Custom/EntityShader"));
        spriteRenderer.material = entityMaterial;
        
        // 设置初始值
        entityMaterial.SetFloat(DefaultAlphaID, defaultAlpha);
        entityMaterial.SetFloat(VisibilityThresholdID, visibilityThreshold);
        entityMaterial.SetInt(LightEnabledID, 1);
        entityMaterial.SetFloat(MaxLightIntensityID, lightRangeHandler != null ? lightRangeHandler.maxLightIntensity : 0.7f);
    }
    
    void Update()
    {
        if (lightRangeHandler != null)
        {
            // 更新着色器中的光照位置和状态
            Vector3 lightPos = lightRangeHandler.transform.position;
            entityMaterial.SetVector(LightPositionID, new Vector4(lightPos.x, lightPos.y, lightPos.z, 1.0f));
            
            // 获取光照半径
            float radius = lightRangeHandler.lightRadius;
            entityMaterial.SetFloat(LightRadiusID, radius);
            
            // 设置光照强度
            entityMaterial.SetFloat(LightIntensityID, lightRangeHandler.GetIntensity());
            
            // 设置光照是否开启
            entityMaterial.SetInt(LightEnabledID, lightRangeHandler.GetStatus() ? 1 : 0);
        }
    }
}