Shader "Easy Color Blind/Visual Color Code/Unlit Texture" {
    Properties {
        _MainTex ("Texture", 2D) = "white" {}
        _Color ("Color", Color) = (1,1,1,1) 
    }
    SubShader {
        Tags { "RenderType"="Opaque" }

        Pass {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            
            #include "UnityCG.cginc"
            #include "CGIncludes/FigureColorFunctions.cginc" 

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
            float4 _Color;

            interpolators vert (meshData v) {
                interpolators o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.screenPos = ComputeScreenPos(o.vertex);
                return o;
            }

            float4 frag (interpolators i) : SV_Target {
                float4 col = tex2D(_MainTex, i.uv) * _Color;
                ApplyFigure(col, i.screenPos);
                return col;
            }
            ENDCG
        }
    }
}
