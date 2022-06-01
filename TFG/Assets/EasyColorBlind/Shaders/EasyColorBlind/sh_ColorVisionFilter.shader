Shader "Easy Color Blind/ColorVisionFilter" {
    Properties {
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader {
        Tags { "RenderType"="Opaque" }

        Pass {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fog
            

            #include "UnityCG.cginc"
            // uber shader
            #pragma multi_compile _CVTYPE_TRICHROMACY _CVTYPE_PROTANOPIA _CVTYPE_DEUTERANOPIA _CVTYPE_TRITANOPIA _CVTYPE_ACHROMATOPSIA

            struct meshData {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct interpolators {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            float _Intensity;

            interpolators vert (meshData v) {
                interpolators o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            float3x3 GetColorBlindnessLinearTransformation(){
            #ifdef _CVTYPE_PROTANOPIA
                float3x3 transformationMatrix =     {   0.0, 2.02344, -2.52581,
                                                        0.0, 1.0, 0.0,
                                                        0.0, 0.0, 1.0
                                                    };
                return transformationMatrix;
            #elif _CVTYPE_DEUTERANOPIA
                float3x3 transformationMatrix =     {   1.0, 0.0, 0.0,
                                                        0.494207, 0.0, 1.24827,
                                                        0.0, 0.0, 1.0
                                                    };
                return transformationMatrix;
            #elif _CVTYPE_TRITANOPIA
                float3x3 transformationMatrix =     {   1.0, 0.0, 0.0,
                                                        0.0, 1.0, 0.0,
                                                        -0.0122449, 0.07203435, 0.0
                                                    };
                return transformationMatrix;
            #else
                float3x3 transformationMatrix =     {   1.0, 0.0, 0.0,
                                                        0.0, 1.0, 0.0,
                                                        0.0, 0.0, 1.0
                                                    };
                return transformationMatrix;
            #endif

            }

            float4 frag (interpolators i) : SV_Target {
                float4 col = tex2D(_MainTex, i.uv);

            #ifdef _CVTYPE_ACHROMATOPSIA
                float l_luminance = col.r * 0.299 + col.g *  0.587 + col.b * 0.114;
                return float4 (l_luminance.xxx, 1);
            #endif
                
                float R = col.r;
                float G = col.g;
                float B = col.b;

                // Manual matrix multiplication
                float L = (17.8824 * R) + (43.5161 * G) + (4.11935 * B);
                float M = (3.45565 * R) + (27.1554 * G) + (3.86714 * B);
                float S = (0.0299566 * R) + (0.184309 * G) + (1.46709 * B);

                float3x3 anomalyTransformationMatrix = GetColorBlindnessLinearTransformation();

                float3 anomalyColorLMS = mul(anomalyTransformationMatrix, float3(L,M,S));      

                // Manual matrix multiplication
                R = (0.0809444479 * anomalyColorLMS.x) + (-0.130504409 * anomalyColorLMS.y) + (0.116721066 * anomalyColorLMS.z);
                G = (-0.0102485335 * anomalyColorLMS.x) + (0.0540193266 * anomalyColorLMS.y) + (-0.113614708 * anomalyColorLMS.z);
                B = (-0.000365296938 * anomalyColorLMS.x) + (-0.00412161469 * anomalyColorLMS.y) + (0.693511405 * anomalyColorLMS.z);

                float3 anomalyColorRGB = lerp(col.rgb, float3(R,G,B), _Intensity);     
                
                return float4(anomalyColorRGB, 1);
            }
            ENDCG
        }
    }
}