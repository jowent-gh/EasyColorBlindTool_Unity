float _IsHighContrast; // can be interpolated to animate transition
float _HCZStart;
float _HCZEnd;
float _HCZIntensity;
float _CamNear;
float _CamFar;
float _HCAlbedo;
float4 _HighContrastColor = (0, 0, 0, 0);
sampler2D _CameraDepthNormalsTexture;

float Lerp(float a, float b, float t){
    return (1.0f - t) * a + b * t;
}

float CustomDepthValue(float4 screenPos){
    // Get Depth 01
    float l_depth = screenPos.z/screenPos.w;

    // Linear Distance Transform
    float l_linearDepth = LinearEyeDepth(l_depth);

    // Remapping 
    return 1 - saturate((l_linearDepth-_HCZStart)/(_HCZEnd-_HCZStart)); 
}

float3 NormalValues(float4 screenPos){
    float l_depth;
    float3 l_normals;
    DecodeDepthNormal(tex2Dproj(_CameraDepthNormalsTexture, screenPos), l_depth, l_normals); 
    
    return l_normals; 
}

#ifdef shader_surf
void SurfaceFinalColor(Input IN, SurfaceOutputStandard o, inout fixed4 color){
    // color = lerp(color, _HighContrastColor, _IsHighContrast);
    float4 l_baseColor = color;
    float l_grayscale = color.r * 0.299 + color.g *  0.587 + color.b * 0.114;
    float4 black = float4(0,0,0,0);
    color = lerp(color, black, _IsHighContrast);
    
    color += _HighContrastColor;
    color -= l_grayscale * _HCAlbedo;
    
    // Additive 
    color += CustomDepthValue(IN.screenPos) * _HCZIntensity;

    // Interpolate with the original
    color = lerp(l_baseColor, color, _IsHighContrast);
}
#endif

void ApplyHighContrastFrag(inout float4 _color, float4 _screenPos){
    float4 l_baseColor = _color;
    float4 l_hcColor = l_baseColor;
    float l_grayscale = l_baseColor.r * 0.299 + l_baseColor.g *  0.587 + l_baseColor.b * 0.114;
    float4 black = float4(0,0,0,0);
    l_hcColor = lerp(l_baseColor, black, _IsHighContrast);
    
    l_hcColor += _HighContrastColor;
    l_hcColor -= l_grayscale * _HCAlbedo;
    
    // Additive 
    l_hcColor += CustomDepthValue(_screenPos) * _HCZIntensity;

    // Interpolate with the original
    _color = lerp(l_baseColor, l_hcColor, _IsHighContrast);
}