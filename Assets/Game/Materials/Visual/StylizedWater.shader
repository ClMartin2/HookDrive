Shader "Custom/StylizedWater"
{
    Properties
    {
        _ShallowColor ("Shallow Color", Color) = (0.325, 0.807, 0.971, 0.725)
        _DeepColor ("Deep Color", Color) = (0.086, 0.407, 1, 0.749)
        _DepthMaxDistance ("Depth Maximum Distance", Float) = 1
        
        _FoamColor ("Foam Color", Color) = (1, 1, 1, 1)
        _FoamDistance ("Foam Distance", Float) = 0.4
        _FoamNoiseTex ("Foam Noise Texture", 2D) = "white" {}
        _FoamNoiseScale ("Foam Noise Scale", Float) = 10
        _FoamSpeed ("Foam Speed", Float) = 0.1
        
        _SurfaceNoise ("Surface Noise", 2D) = "white" {}
        _SurfaceNoiseScale ("Surface Noise Scale", Float) = 0.5
        _SurfaceNoiseSpeed ("Surface Noise Speed", Vector) = (0.03, 0.03, 0, 0)
        _SurfaceDistortion ("Surface Distortion", Range(0, 1)) = 0.27
        
        _WaveHeight ("Wave Height", Float) = 0.2
        _WaveSpeed ("Wave Speed", Float) = 1
        _WaveFrequency ("Wave Frequency", Float) = 2
        
        _Smoothness ("Smoothness", Range(0, 1)) = 0.9
        _Specular ("Specular", Range(0, 1)) = 0.7
    }
    
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        LOD 200
        
        CGPROGRAM
        #pragma surface surf StandardSpecular alpha vertex:vert
        #pragma target 3.0
        
        sampler2D _CameraDepthTexture;
        sampler2D _SurfaceNoise;
        sampler2D _FoamNoiseTex;
        
        float4 _ShallowColor;
        float4 _DeepColor;
        float _DepthMaxDistance;
        
        float4 _FoamColor;
        float _FoamDistance;
        float _FoamNoiseScale;
        float _FoamSpeed;
        
        float _SurfaceNoiseScale;
        float2 _SurfaceNoiseSpeed;
        float _SurfaceDistortion;
        
        float _WaveHeight;
        float _WaveSpeed;
        float _WaveFrequency;
        
        float _Smoothness;
        float _Specular;
        
        struct Input
        {
            float2 uv_SurfaceNoise;
            float4 screenPos;
            float3 worldPos;
            float3 viewDir;
        };
        
        void vert(inout appdata_full v)
        {
            float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
            
            // Vagues sinusoïdales
            float wave1 = sin(worldPos.x * _WaveFrequency + _Time.y * _WaveSpeed) * _WaveHeight;
            float wave2 = sin(worldPos.z * _WaveFrequency * 0.7 + _Time.y * _WaveSpeed * 0.8) * _WaveHeight * 0.5;
            
            v.vertex.y += wave1 + wave2;
        }
        
        void surf (Input IN, inout SurfaceOutputStandardSpecular o)
        {
            // Depth-based color
            float existingDepth01 = tex2Dproj(_CameraDepthTexture, UNITY_PROJ_COORD(IN.screenPos)).r;
            float existingDepthLinear = LinearEyeDepth(existingDepth01);
            float depthDifference = existingDepthLinear - IN.screenPos.w;
            float waterDepthDifference = saturate(depthDifference / _DepthMaxDistance);
            float4 waterColor = lerp(_ShallowColor, _DeepColor, waterDepthDifference);
            
            // Surface noise pour distortion
            float2 noiseUV = IN.worldPos.xz * _SurfaceNoiseScale + _SurfaceNoiseSpeed * _Time.y;
            float surfaceNoise = tex2D(_SurfaceNoise, noiseUV).r;
            float surfaceNoise2 = tex2D(_SurfaceNoise, noiseUV * 0.7 + float2(0.5, 0.5)).r;
            float combinedNoise = (surfaceNoise + surfaceNoise2) * 0.5;
            
            // Foam
            float foamDepth = saturate(depthDifference / _FoamDistance);
            float2 foamNoiseUV = IN.worldPos.xz * _FoamNoiseScale + _Time.y * _FoamSpeed;
            float foamNoise = tex2D(_FoamNoiseTex, foamNoiseUV).r;
            float foamLine = step(foamNoise, 1 - foamDepth);
            
            // Combiner couleur et foam
            float4 finalColor = lerp(_FoamColor, waterColor, foamLine);
            
            // Normal distorsion pour effet stylisé
            float3 normal = float3(
                (surfaceNoise - 0.5) * _SurfaceDistortion,
                1,
                (surfaceNoise2 - 0.5) * _SurfaceDistortion
            );
            normal = normalize(normal);
            
            o.Albedo = finalColor.rgb;
            o.Normal = normal;
            o.Specular = _Specular;
            o.Smoothness = _Smoothness;
            o.Alpha = finalColor.a;
        }
        ENDCG
    }
    
    FallBack "Transparent/Diffuse"
}