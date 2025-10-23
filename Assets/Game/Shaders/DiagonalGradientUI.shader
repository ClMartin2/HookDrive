Shader "Custom/DiagonalGradient"
{
    Properties
    {
        _ColorA("Color A", Color) = (1,1,0,1)
        _ColorB("Color B", Color) = (1,0.5,0,1)
        _MainTex("Main Texture", 2D) = "white" {} // <-- ajouté
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

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

            sampler2D _MainTex; // <-- ajouté pour éviter l'erreur
            float4 _ColorA;
            float4 _ColorB;

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float diag = (i.uv.x + i.uv.y) / 2.0;
                fixed4 col = lerp(_ColorA, _ColorB, diag);
                return col;
            }
            ENDCG
        }
    }
}
