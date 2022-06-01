using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[ExecuteAlways]
public class ColorReader : MonoBehaviour {
    [SerializeField]
    Image m_target;
    
    [SerializeField]
    Image m_figure;

    [SerializeField]
    Image m_redFigure, m_greenFigure, m_blueFigure;

    [SerializeField]
    Image m_valueFigure;
    
    [SerializeField]
    float m_figureOffset = 120;

    RenderTexture m_rt;
    bool m_blit = false;
    
    [SerializeField]
    float m_figureDamp = 20;

    ColorFigureTextures m_textures;

    void UpdateTextures(){
        if(ColorFigureConfiguration.m_instance == null) return;
        if(ColorFigureConfiguration.m_instance.m_textures == null) return;
        
        ColorFigureTextures l_currentTexs = m_textures;
        m_textures = ColorFigureConfiguration.m_instance.m_textures;
        if(l_currentTexs == m_textures) return;

        Sprite m_sprite = Sprite.Create(m_textures.m_crosshair, new Rect(0.0f, 0.0f, m_textures.m_crosshair.width, m_textures.m_crosshair.height), 
                                        new Vector2(0.5f, 0.5f), 100.0f);
        m_target.sprite = m_sprite;
        
        m_sprite = Sprite.Create(m_textures.m_background, new Rect(0.0f, 0.0f, m_textures.m_background.width, m_textures.m_background.height), 
                                        new Vector2(0.5f, 0.5f), 100.0f);
        m_figure.sprite = m_sprite;
        
        m_sprite = Sprite.Create(m_textures.m_red, new Rect(0.0f, 0.0f, m_textures.m_red.width, m_textures.m_red.height), 
                                        new Vector2(0.5f, 0.5f), 100.0f);
        m_redFigure.sprite = m_sprite;
        
        m_sprite = Sprite.Create(m_textures.m_green, new Rect(0.0f, 0.0f, m_textures.m_green.width, m_textures.m_green.height), 
                                        new Vector2(0.5f, 0.5f), 100.0f);
        m_greenFigure.sprite = m_sprite;
        
        m_sprite = Sprite.Create(m_textures.m_blue, new Rect(0.0f, 0.0f, m_textures.m_blue.width, m_textures.m_blue.height), 
                                        new Vector2(0.5f, 0.5f), 100.0f);
        m_blueFigure.sprite = m_sprite;
        
        m_sprite = Sprite.Create(m_textures.m_lightDark, new Rect(0.0f, 0.0f, m_textures.m_lightDark.width, m_textures.m_lightDark.height), 
                                        new Vector2(0.5f, 0.5f), 100.0f);
        m_valueFigure.sprite = m_sprite;

        m_target.SetNativeSize();
        m_figure.SetNativeSize();
        m_redFigure.SetNativeSize();
        m_greenFigure.SetNativeSize();
        m_blueFigure.SetNativeSize();
        m_valueFigure.SetNativeSize();
    }
    
    private void Update() {
        ApplyFilter[] l_filters = Camera.main.GetComponents<ApplyFilter>();  
        if(l_filters.Length > 0){
            m_rt = l_filters[0].GetUnfilteredTexture();
            m_blit = false;
        }
        else{
            if(m_rt == null)
                m_rt = new RenderTexture(Screen.width, Screen.height, 0);
            m_blit = true;
        }

        UpdateColor(); 
        UpdateFigurePosition();
        UpdateTextures();
    }

    void UpdateFigurePosition(){
        Vector3 l_centerScreen = new Vector3(Screen.width/2.0f, Screen.height/2.0f, 0);
        Vector3 l_centerScreenOffset = Input.mousePosition - l_centerScreen;

        l_centerScreenOffset.x = Mathf.Clamp(l_centerScreenOffset.x, -1, 1);
        l_centerScreenOffset.y = Mathf.Clamp(l_centerScreenOffset.y, -1, 1);

        l_centerScreenOffset *= m_figureOffset;

        Vector3 l_figureTargetPosition = m_target.transform.position - l_centerScreenOffset;
        m_figure.transform.position = Vector3.Lerp(m_figure.transform.position, l_figureTargetPosition, Time.deltaTime * m_figureDamp);
    }

    void UpdateColor(){
        Color l_col = ColorFromRT(m_rt, new Vector2(m_target.transform.position.x, m_target.transform.position.y)); 
        ColorFigure l_colorFigureNodes;
        if(ColorFigureConfiguration.m_instance == null)
            l_colorFigureNodes = EasyColorblindFuncions.ColorToFigure(l_col, .6f, .2f); 
        else
            l_colorFigureNodes = EasyColorblindFuncions.ColorToFigure(l_col, ColorFigureConfiguration.m_instance.m_lightDarkThreshold,
                                                                             ColorFigureConfiguration.m_instance.m_fullBlackWhiteThreshold); 


        m_redFigure.gameObject.SetActive(FloatToBool(l_colorFigureNodes.red));
        m_greenFigure.gameObject.SetActive(FloatToBool(l_colorFigureNodes.green));
        m_blueFigure.gameObject.SetActive(FloatToBool(l_colorFigureNodes.blue));
        
        Color l_valueCol = Color.white;
        switch(l_colorFigureNodes.sv){
            case 0:
                l_valueCol.a = 0;
                m_valueFigure.color = l_valueCol;
                break;
            case 1: // Light
                m_valueFigure.color = l_valueCol;
                break;
            case 2: // Dark
                l_valueCol = Color.black;
                m_valueFigure.color = l_valueCol;
                break;
        }
    }

    bool FloatToBool(float _float) => _float == 1 ? true : false;

    Color ColorFromRT(RenderTexture _rt, Vector2 _coordinates) {
        Texture2D tex = new Texture2D(1, 1, TextureFormat.RGB24, false);
        
        RenderTexture.active = _rt;
        float l_coordX = Mathf.Clamp(_coordinates.x, 1, Screen.width-1);
        float l_coordY = Mathf.Clamp(Screen.height - _coordinates.y, 1, Screen.height-1);
        tex.ReadPixels(new Rect(l_coordX, l_coordY, 1, 1), 0, 0);        
        tex.Apply();
        
        return tex.GetPixel(0, 0);
    }   

    private void OnRenderImage(RenderTexture src, RenderTexture dest) {
        if(m_blit)
            Graphics.Blit(src, m_rt);
    }

    public void SetComponents(Image _target, Image _background, Image _red, Image _green, Image _blue, Image _sv){
        m_target = _target;
        m_figure = _background;
        m_redFigure = _red;
        m_greenFigure = _green;
        m_blueFigure = _blue;
        m_valueFigure = _sv;

        UpdateTextures();
    }
}