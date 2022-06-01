float _HasColorFigure; // can be interpolated to animate transition
sampler2D _ColorFigureBackground;
sampler2D _ColorFigureRed;
sampler2D _ColorFigureGreen;
sampler2D _ColorFigureBlue;
sampler2D _ColorFigureSV;
float4 _FigureCoords;
float4 _FigureNodes; // RGB + SV Additions

float4 AlphaBlend(float4 _src, float4 _dst){ 
    return _src * _src.a + _dst * (1-_src.a);
}

#ifdef shader_surf
void SurfaceFinalColor(Input IN, SurfaceOutputStandard o, inout fixed4 color){
    float4 l_baseColor = color;
    float2 l_screenUVs = IN.screenPos.xy/IN.screenPos.w;

    float2 l_remappedUVs = float2((l_screenUVs.x - _FigureCoords.x) / (_FigureCoords.z - _FigureCoords.x),
                                  (l_screenUVs.y - _FigureCoords.y) / (_FigureCoords.w - _FigureCoords.y)); 

    float l_inside01 = l_remappedUVs.x > 0 && l_remappedUVs.y > 0 && l_remappedUVs.x < 1 && l_remappedUVs.y < 1;

    float4 l_bg = tex2D(_ColorFigureBackground, l_remappedUVs);
    float4 l_r = tex2D(_ColorFigureRed, l_remappedUVs);
    float4 l_g = tex2D(_ColorFigureGreen, l_remappedUVs);
    float4 l_b = tex2D(_ColorFigureBlue, l_remappedUVs);
    float4 l_sv = tex2D(_ColorFigureSV, l_remappedUVs);
    l_sv.rbg *= (_FigureNodes.a == 2) ? 0 : 1;


    color = AlphaBlend(l_bg, color); 
    color = AlphaBlend(l_r * _FigureNodes.r, color);  
    color = AlphaBlend(l_g * _FigureNodes.g, color); 
    color = AlphaBlend(l_b * _FigureNodes.b, color); 
    color = AlphaBlend(l_sv * (_FigureNodes.a != 0), color); 

    color = lerp(l_baseColor, color, _HasColorFigure);
}
#endif

void ApplyFigure(inout float4 _color, float4 _screenPos){
    float4 l_baseColor = _color;
    float2 l_screenUVs = _screenPos.xy/_screenPos.w;

    float2 l_remappedUVs = float2((l_screenUVs.x - _FigureCoords.x) / (_FigureCoords.z - _FigureCoords.x),
                                  (l_screenUVs.y - _FigureCoords.y) / (_FigureCoords.w - _FigureCoords.y)); 

    float l_inside01 = l_remappedUVs.x > 0 && l_remappedUVs.y > 0 && l_remappedUVs.x < 1 && l_remappedUVs.y < 1;

    float4 l_bg = tex2D(_ColorFigureBackground, l_remappedUVs);
    float4 l_r = tex2D(_ColorFigureRed, l_remappedUVs);
    float4 l_g = tex2D(_ColorFigureGreen, l_remappedUVs);
    float4 l_b = tex2D(_ColorFigureBlue, l_remappedUVs);
    float4 l_sv = tex2D(_ColorFigureSV, l_remappedUVs);
    l_sv.rbg *= (_FigureNodes.a == 2) ? 0 : 1;


    _color = AlphaBlend(l_bg, _color); 
    _color = AlphaBlend(l_r * _FigureNodes.r, _color); 
    _color = AlphaBlend(l_g * _FigureNodes.g, _color); 
    _color = AlphaBlend(l_b * _FigureNodes.b, _color); 
    _color = AlphaBlend(l_sv * (_FigureNodes.a != 0), _color); 

    _color = lerp(l_baseColor, _color, _HasColorFigure);
}
