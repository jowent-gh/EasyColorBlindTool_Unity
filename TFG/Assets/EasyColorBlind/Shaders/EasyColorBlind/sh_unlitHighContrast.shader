Shader "Easy Color Blind/High Contrast/Unlit High Contrast" {
    Properties {
        _MainTex ("Texture", 2D) = "white" {}
        _HighContrastColor("High Contrast Color", Color) = (0,0,0,0)
    }
    SubShader {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc" 
            #include "CGIncludes/HighContrastFunctions.cginc"

            struct meshData {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct interpolators {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
                float4 screenPos : TEXCOORD1;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            interpolators vert (meshData v) {
                interpolators o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.screenPos = ComputeScreenPos(o.vertex);
                return o;
            }

            fixed4 frag (interpolators i) : SV_Target {
                fixed4 col = tex2D(_MainTex, i.uv);
                
                // Apply High Contrast with the final color
                ApplyHighContrastFrag(col, i.screenPos);

                return col;
            }
            ENDCG
        }
    }
}
