Shader "Easy Color Blind/Outline High Contrast" { // "Hidden/"
    Properties {
        _MainTex ("Texture", 2D) = "white" {}
        _DepthThreshold("Depth Threshold", Range(0, 1)) = 0.5
        _NormalThreshold("Normal Threshold", Range(0, 2.5)) = 0.5
        _OutlineAlpha("Outline Alpha", Range(0, 1)) = 1
        _Thickness("Outline Thickness", Range(1, 5)) = 1
    }
    SubShader {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct meshData {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct interpolators {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            sampler2D _MainTex;
            sampler2D _CameraDepthNormalsTexture;
            float _DepthThreshold;
            float _NormalThreshold;
            float _OutlineAlpha;
            float _Thickness;
            float _IsHighContrast;
            float _HasOutline;
            
            interpolators vert (meshData v) {
                interpolators o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            float4 frag (interpolators i) : SV_Target {
                float4 l_baseColor = tex2D(_MainTex, i.uv);
                
                if(_HasOutline == 0) return l_baseColor;

                //
                // Sample Diagonal Values
                //

                float2 l_uvs = i.uv;
                float2 l_uvPixelSize = float2(1.0/_ScreenParams.x, 1.0/_ScreenParams.y) * _Thickness;
                
                // Top Right
                float l_depthTR;
                float3 l_normalsTR;
                l_uvs = i.uv + l_uvPixelSize; // 1, -1
                DecodeDepthNormal(tex2D(_CameraDepthNormalsTexture, l_uvs), l_depthTR, l_normalsTR); 
                
                // Top Left
                float l_depthTL;
                float3 l_normalsTL;
                l_uvs = i.uv + float2(-l_uvPixelSize.x, l_uvPixelSize.y); // -1, 1
                DecodeDepthNormal(tex2D(_CameraDepthNormalsTexture, l_uvs), l_depthTL, l_normalsTL); 
                
                // Bottom Right
                float l_depthBR;
                float3 l_normalsBR;
                l_uvs = i.uv + float2(l_uvPixelSize.x, -l_uvPixelSize.y); // 1, -1
                DecodeDepthNormal(tex2D(_CameraDepthNormalsTexture, l_uvs), l_depthBR, l_normalsBR); 
                
                // Bottom Left
                float l_depthBL;
                float3 l_normalsBL;
                l_uvs = i.uv - l_uvPixelSize; // -1, -1
                DecodeDepthNormal(tex2D(_CameraDepthNormalsTexture, l_uvs), l_depthBL, l_normalsBL); 
                
                //
                // Calculate Differences
                //

                // Depth
                float l_depthDifference0 = abs(l_depthTR - l_depthBL);
                float l_depthDifference1 = abs(l_depthTL - l_depthBR);
                float l_depthEdge = (l_depthDifference0 + l_depthDifference1) * 10 > _DepthThreshold;
                
                // Normals
                float3 l_normalDifference0 = l_normalsTR - l_normalsBL;
                float3 l_normalDifference1 = l_normalsTL - l_normalsBR;
                float l_normalEdge = (length(l_normalDifference0) + length(l_normalDifference1)) > _NormalThreshold;

                float l_edge = saturate(l_depthEdge + l_normalEdge);
                

                float4 l_colorEdgeApplied = l_baseColor + l_edge * _OutlineAlpha;

                return lerp(l_baseColor, l_colorEdgeApplied, _IsHighContrast);    
            }
            ENDCG
        }
    }
}
