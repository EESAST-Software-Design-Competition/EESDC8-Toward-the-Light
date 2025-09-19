Shader "Custom/TreeShadowShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Color ("Color", Color) = (0,0,0,0.6)
        _FadeStart ("Fade Start", Range(0, 1)) = 0.8
        _FadeEnd ("Fade End", Range(0, 1)) = 1.0
    }
    
    SubShader
    {
        Tags {"Queue"="Transparent" "RenderType"="Transparent"}
        LOD 100
        
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            
            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };
            
            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };
            
            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _Color;
            float _FadeStart;
            float _FadeEnd;
            
            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }
            
            fixed4 frag (v2f i) : SV_Target
            {
                // Sample texture
                fixed4 col = tex2D(_MainTex, i.uv) * _Color;
                
                // Fade out at the end of the shadow
                float fadeOut = 1.0;
                if (i.uv.x > _FadeStart)
                {
                    fadeOut = 1.0 - smoothstep(_FadeStart, _FadeEnd, i.uv.x);
                }
                
                col.a *= fadeOut;
                return col;
            }
            ENDCG
        }
    }
}
