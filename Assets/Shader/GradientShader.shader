Shader "UI/GradientShader" {
    Properties {
        _Color1 ("Color 1", Color) = (1, 1, 0, 1)  // Màu vàng (RGBA)
        _Color2 ("Color 2", Color) = (1, 1, 1, 1)  // Màu trắng (RGBA)
    }
    SubShader {
        Tags { "Queue" = "Overlay" "IgnoreProjector" = "True" "RenderType" = "Transparent" }
        Pass {
            ZWrite Off
            Blend SrcAlpha OneMinusSrcAlpha

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata_t {
                float4 vertex : POSITION;
                float2 texcoord : TEXCOORD0;
            };

            struct v2f {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            fixed4 _Color1;
            fixed4 _Color2;

            v2f vert(appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.texcoord;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                // Áp dụng gradient theo trục Y
                return lerp(_Color1, _Color2, i.uv.y);
            }
            ENDCG
        }
    }
}
