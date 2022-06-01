Shader "Easy Color Blind/High Contrast/Surface High Contrast" {
    Properties {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0
        _HighContrastColor("High Contrast Color", Color) = (0,0,0,0)
    }
    SubShader {
        Tags { "RenderType"="Opaque" }
        // LOD 200
  
        CGPROGRAM
        #pragma surface surf Standard finalcolor:SurfaceFinalColor
        
        #pragma target 3.0


        sampler2D _MainTex;

        struct Input { 
            float2 uv_MainTex;
            float4 screenPos; 
        };

        half _Glossiness;
        half _Metallic;
        fixed4 _Color;
        fixed4 _Normal;


        UNITY_INSTANCING_BUFFER_START(Props)
        UNITY_INSTANCING_BUFFER_END(Props)

        // 
        // Easy Color Blind Visual Color Code
        //
        // Requirements: 
        // finalColor:SurfaceFinalColor on #pragma surface surf Standard
        // add float4 screenPos; inside Input struct
        // #define shader_surf
        // #include "CGIncludes/HighContrastFunctions.cginc" after Input struct definition
        //

        #define shader_surf
        #include "CGIncludes/HighContrastFunctions.cginc"

        void surf (Input IN, inout SurfaceOutputStandard o)  {
            fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
            o.Albedo = c.rgb;
            o.Metallic = _Metallic; 
            o.Smoothness = _Glossiness;
            o.Alpha = c.a;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
