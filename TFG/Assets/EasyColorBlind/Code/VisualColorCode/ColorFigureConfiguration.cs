using UnityEngine;

[ExecuteAlways]
public class ColorFigureConfiguration : MonoBehaviour {
    
    public static ColorFigureConfiguration m_instance;

    public ColorFigureTextures m_textures;
    
    [Range(0, 1)] public float m_lightDarkThreshold = .6f;
    [Range(0, 1)] public float m_fullBlackWhiteThreshold = .2f;

    private void OnEnable() {
        if(m_instance == null)
            m_instance = this;
        else
            Destroy(gameObject);
    }

    private void Update() {
        Shader.SetGlobalTexture("_ColorFigureRed", m_textures.m_red);
        Shader.SetGlobalTexture("_ColorFigureGreen", m_textures.m_green);
        Shader.SetGlobalTexture("_ColorFigureBlue", m_textures.m_blue);
        Shader.SetGlobalTexture("_ColorFigureSV", m_textures.m_lightDark);
        Shader.SetGlobalTexture("_ColorFigureBackground", m_textures.m_background);
    }
}
