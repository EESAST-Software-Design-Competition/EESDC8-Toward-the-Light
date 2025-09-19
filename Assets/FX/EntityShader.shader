Shader "Custom/EntityShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _DefaultAlpha ("Default Alpha", Range(0, 1)) = 0.4
        _VisibilityThreshold ("Visibility Threshold", Range(0, 1)) = 0.25 // 默认为1/4, 与黑云的反向效果
    }
    
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        LOD 100
        
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        Cull Off
        
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_instancing
            #include "UnityCG.cginc"
            
            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                fixed4 color : COLOR;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };
            
            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                fixed4 color : COLOR;
                float4 worldPos : TEXCOORD1;
                UNITY_VERTEX_OUTPUT_STEREO
            };
            
            sampler2D _MainTex;
            float4 _MainTex_ST;
            float _DefaultAlpha;
            float _VisibilityThreshold; // 可见度阈值
            
            // 定义光源位置和强度，这些将从C#脚本设置
            float4 _LightPosition;
            float _LightRadius;
            float _LightIntensity;
            float _MaxLightIntensity; // 最大光照强度
            int _LightEnabled;
            
            v2f vert (appdata v)
            {
                v2f o;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.color = v.color;
                o.worldPos = mul(unity_ObjectToWorld, v.vertex);
                return o;
            }
            
            fixed4 frag (v2f i) : SV_Target
            {
                // 从纹理采样
                fixed4 col = tex2D(_MainTex, i.uv) * i.color;
                
                // 计算距离光源的距离
                float distance = length(i.worldPos.xy - _LightPosition.xy);
                
                // 计算光照强度
                float lightFactor = 0;
                if (_LightEnabled > 0) {
                    lightFactor = 1.0 - saturate(distance / _LightRadius);
                    lightFactor = lightFactor * _LightIntensity;
                    
                    // 计算相对于最大强度的比例
                    float relativeIntensity = lightFactor / _MaxLightIntensity;
                    
                    // 当相对强度达到或超过阈值时，变为完全可见
                    if (relativeIntensity >= _VisibilityThreshold) {
                        lightFactor = 1.0; // 使完全可见
                    } else {
                        // 映射到0-1范围，以便在达到阈值前平滑过渡
                        lightFactor = relativeIntensity / _VisibilityThreshold;
                    }
                }
                
                // 根据光照强度调整透明度 - 与黑云相反，这里是增加透明度
                float finalAlpha = _DefaultAlpha + (1.0 - _DefaultAlpha) * lightFactor;
                
                // 当光照强度较高时，实体逐渐变得更加可见
                col.a *= finalAlpha;
                
                return col;
            }
            ENDCG
        }
    }
}