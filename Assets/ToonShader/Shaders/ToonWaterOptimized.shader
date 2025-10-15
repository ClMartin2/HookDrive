Shader "Roystan/Toon/WaterTutOptimized"
{
    Properties
    {
        _DepthGradientShallow("Shallow Color", Color) = (0.325, 0.807, 0.971, 0.725)
        _DepthGradientDeep("Deep Color", Color) = (0.086, 0.407, 1, 0.749)
        _DepthMaxDistance("Max Depth Distance", Float) = 1.0

        _FoamColor("Foam Color", Color) = (1,1,1,1)
        _FoamMaxDistance("Foam Max Distance", Float) = 0.4
        _FoamMinDistance("Foam Min Distance", Float) = 0.04

        _SurfaceNoise("Surface Noise", 2D) = "white" {}
        _SurfaceNoiseScroll("Noise Scroll", Vector) = (0.03, 0.03, 0, 0)

        _SurfaceDistortion("Surface Distortion", 2D) = "white" {}
        _SurfaceDistortionAmount("Distortion Amount", Range(0,1)) = 0.27
    }

    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            #define SMOOTHSTEP_AA 0.01

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 noiseUV : TEXCOORD0;
                float2 distortUV : TEXCOORD1;
                float3 worldPos : TEXCOORD2;
            };

            sampler2D _SurfaceNoise;
            float4 _SurfaceNoise_ST;

            sampler2D _SurfaceDistortion;
            float4 _SurfaceDistortion_ST;

            float4 _DepthGradientShallow;
            float4 _DepthGradientDeep;
            float4 _FoamColor;

            float _DepthMaxDistance;
            float _FoamMaxDistance;
            float _FoamMinDistance;
            float _SurfaceDistortionAmount;
            float2 _SurfaceNoiseScroll;

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                o.distortUV = TRANSFORM_TEX(v.uv, _SurfaceDistortion);
                o.noiseUV = TRANSFORM_TEX(v.uv, _SurfaceNoise);
                return o;
            }

            float4 alphaBlend(float4 top, float4 bottom)
            {
                float3 color = (top.rgb * top.a) + (bottom.rgb * (1 - top.a));
                float alpha = top.a + bottom.a * (1 - top.a);
                return float4(color, alpha);
            }

            float4 frag(v2f i) : SV_Target
            {
                // Approximation depth par Y local
                float depth01 = saturate((i.worldPos.y + _DepthMaxDistance*0.5) / _DepthMaxDistance);
                float4 waterColor = lerp(_DepthGradientShallow, _DepthGradientDeep, depth01);

                // Distorsion et scrolling du bruit
                float2 distortSample = (tex2D(_SurfaceDistortion, i.distortUV).xy * 2 - 1) * _SurfaceDistortionAmount;
                float2 noiseUV = i.noiseUV + _Time.y * _SurfaceNoiseScroll.xy + distortSample;
                float surfaceNoiseSample = tex2D(_SurfaceNoise, noiseUV).r;

                // Foam basé sur Y local + noise
                float foamFactor = smoothstep(_FoamMinDistance, _FoamMaxDistance, 1.0 - depth01) * surfaceNoiseSample;
                float4 foamColor = _FoamColor;
                foamColor.a *= foamFactor;

                return alphaBlend(foamColor, waterColor);
            }
            ENDCG
        }
    }
}
