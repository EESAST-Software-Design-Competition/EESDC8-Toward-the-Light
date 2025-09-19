using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class BlackCloudController : MonoBehaviour
{
    private ParticleSystem particleSystem;
    private Material particleMaterial;
    private BoxCollider2D _bc;
    
    [SerializeField] private LightRangeHandler lightRangeHandler;
    [SerializeField] private float defaultAlpha = 1.0f;
    
    // 着色器相关属性
    private static readonly int LightPositionID = Shader.PropertyToID("_LightPosition");
    private static readonly int LightRadiusID = Shader.PropertyToID("_LightRadius");
    private static readonly int LightIntensityID = Shader.PropertyToID("_LightIntensity");
    private static readonly int LightEnabledID = Shader.PropertyToID("_LightEnabled");
    private static readonly int AlphaID = Shader.PropertyToID("_Alpha");
    private static readonly int MaxLightIntensityID = Shader.PropertyToID("_MaxLightIntensity");
    private static readonly int TransparencyThresholdID = Shader.PropertyToID("_TransparencyThreshold");
    
    void Start()
    {
        // 获取组件
        particleSystem = GetComponent<ParticleSystem>();
        _bc = GetComponent<BoxCollider2D>();
        // 获取粒子系统的材质
        var renderer = particleSystem.GetComponent<ParticleSystemRenderer>();
        particleMaterial = renderer.material;
        particleMaterial.SetInt(LightEnabledID, 1);
        // 设置初始值
        particleMaterial.SetFloat(AlphaID, defaultAlpha);
        _bc.size = new Vector2(particleSystem.shape.scale.x, particleSystem.shape.scale.y - .01f);
        particleMaterial.SetFloat(MaxLightIntensityID, lightRangeHandler.maxLightIntensity);
        particleMaterial.SetFloat(TransparencyThresholdID, .75f);
    }
    
    void Update()
    {
        if (lightRangeHandler != null)
        {
            // 更新着色器中的光照位置和状态
            Vector3 lightPos = lightRangeHandler.transform.position;
            particleMaterial.SetVector(LightPositionID, new Vector4(lightPos.x, lightPos.y, lightPos.z, 1.0f));
            
            // 获取光照半径（假设LightRangeHandler有一个获取半径的方法）
            float radius = lightRangeHandler.lightRadius;
            particleMaterial.SetFloat(LightRadiusID, radius);
            
            // 设置光照强度
            particleMaterial.SetFloat(LightIntensityID, lightRangeHandler.GetIntensity());
            
            // 设置光照是否开启
            particleMaterial.SetInt(LightEnabledID, 1);
        }
    }
    
}
