Shader "Easy Color Blind/High Contrast/Skybox High Contrast"
{
    Properties {
        _MainTex ("HDRI Texture", 2D) = "white" {}
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

            struct MeshData {
                float4 vertex : POSITION;
                float3 viewDir : TEXCOORD0;
            };

            struct v2f{
                float4 vertex : SV_POSITION;
                float3 viewDir : TEXCOORD0;
                float4 screenPos : TEXCOORD1;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            #define TAU 6.28318530718

            v2f vert (MeshData v){
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.viewDir = v.viewDir;
                o.screenPos = ComputeScreenPos(o.vertex);
                return o;
            }

            float2 DirToRectilinear(float3 dir){
                float x = atan2(dir.z, dir.x) / TAU + 0.5; // from -TAU/2 TAU/2 to 0 1
                float y = dir.y * 0.5 + 0.5;
                return float2(x,y);
            }

            float4 frag (v2f i) : SV_Target{
                float4 col = tex2Dlod(_MainTex, float4( DirToRectilinear(i.viewDir), 0, 0));
                col.a = 1;
                
                // Apply High Contrast with the final color
                ApplyHighContrastFrag(col, i.screenPos);
                
                return col;
            }
            ENDCG
        }
    }
}
