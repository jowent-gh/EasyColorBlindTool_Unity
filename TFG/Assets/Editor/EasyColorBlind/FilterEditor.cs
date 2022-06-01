using UnityEngine;
using UnityEditor;
#if UNITY_EDITOR
public class FilterEditor : EditorWindow {
    
    static bool m_enabled = false; 
    static float m_severityValue = 0;
    static Texture2D m_iconNormal, m_iconProtanopia, m_iconDeuteranopia, m_iconTritanopia, m_iconAchromatopsia; 
    static Texture2D m_buttonNormal, m_buttonSelected, m_buttonHover; 
    static Texture2D m_buttonInfo; 
    static Texture2D m_boxTexture; 
    static Texture2D m_sliderRail; 
    static Material m_filterMaterial;
    const string m_intensityProperty = "_Intensity";
    static int m_selectedButton = 0;
    static bool m_hidden = true;

    static string m_tooltip = "N: Normal Vision / Trichromacy\n\nP: Protanomaly / Protanopia - Partial/Full lost of red cones\n\nD: Deuteranomaly / Deuteranopia - Partial/Full lost of green cones\n\nT: Protanomaly / Protanopia - Partial/Full lost of blue cones\n\nA: Achromatopsia - Lost of cons - Vision based on Rods";
    const string m_tag = "FilterTool";


    [MenuItem("Easy Color Blind/Editor Filter/Enable")]
    public static void Enable(){
        if(m_enabled) return;
        SceneView.duringSceneGui += OnSceneGUI;
        m_enabled = true;
    }

    [MenuItem("Easy Color Blind/Editor Filter/Disable")]
    public static void Disable(){
        if(!m_enabled) return;
        SceneView.duringSceneGui -= OnSceneGUI;
        m_enabled = false;
    }

    [UnityEditor.Callbacks.DidReloadScripts]
    private static void OnScriptsReloaded() {
        LoadTextures();
        LoadFilterMaterial();
        SetComponentLastInCamera();
        Enable();
    }

    private static void LoadTextures(){
        m_iconNormal = Resources.Load<Texture2D>("EasyColorBlind/icon_normal");
        m_iconProtanopia = Resources.Load<Texture2D>("EasyColorBlind/icon_protanopia");
        m_iconDeuteranopia = Resources.Load<Texture2D>("EasyColorBlind/icon_deuteranopia");
        m_iconTritanopia = Resources.Load<Texture2D>("EasyColorBlind/icon_tritanopia");
        m_iconAchromatopsia = Resources.Load<Texture2D>("EasyColorBlind/icon_achromatopsia");
        m_buttonNormal = Resources.Load<Texture2D>("EasyColorBlind/button_normal");
        m_buttonHover = Resources.Load<Texture2D>("EasyColorBlind/button_hover");
        m_buttonSelected = Resources.Load<Texture2D>("EasyColorBlind/button_selected");
        m_boxTexture = Resources.Load<Texture2D>("EasyColorBlind/box_bg");
        m_buttonInfo = Resources.Load<Texture2D>("EasyColorBlind/button_info");
        m_sliderRail = Resources.Load<Texture2D>("EasyColorBlind/slider_rail");
    }

    private static void LoadFilterMaterial(){
        m_filterMaterial = Resources.Load<Material>("EasyColorBlind/mat_ColorVisionFilter");
    }

    private static void OnSceneGUI(SceneView sceneView){
        
        // Avoid Compile for first time issues
        if(m_boxTexture == null || m_filterMaterial == null || Camera.main.GetComponent<ApplyFilter>() == null){
            LoadTextures();
            LoadFilterMaterial();
            SetComponentLastInCamera();
        }

        Handles.BeginGUI();

        // Get Scene View Information
        Vector2 l_screenDimensions = new Vector2(Screen.width, Screen.height); 

        GameObject l_sceneCamObj = GameObject.Find( "SceneCamera" );
        Rect l_sceneRect = l_sceneCamObj != null ? l_sceneCamObj.GetComponent<Camera>().pixelRect : new Rect();

        float l_buttonHeight = l_screenDimensions.y / 20;
        float l_buttonWidth = l_buttonHeight;

        if(m_hidden) {
            Rect l_hideButtonRect = new Rect(0, l_sceneRect.height - l_buttonHeight *0.5f, l_buttonWidth / 2, l_buttonHeight / 2);            

            if(GUI.Button(l_hideButtonRect, "▲",  ButtonStyle(-1)))
                m_hidden = !m_hidden;          

            l_hideButtonRect.x = l_buttonWidth/2;
            GUI.DrawTexture(l_hideButtonRect, m_iconNormal);
            
            Handles.EndGUI(); 
            return;
        }
        else{
            Rect l_hideButtonRect = new Rect(0, l_sceneRect.height - (l_buttonHeight + l_buttonHeight * 1.5f), 
                                             l_buttonWidth /2, l_buttonHeight /2);
                                            
            if(GUI.Button(l_hideButtonRect, "▼", ButtonStyle(-1)))
                m_hidden = !m_hidden;                             

            GUIStyle l_infoButtonStyle = new GUIStyle();
            l_infoButtonStyle.normal.background = m_buttonNormal;                                            
            
            GUIContent l_infoContent = new GUIContent();
            l_infoContent.image = m_buttonInfo;
            l_infoContent.tooltip = m_tooltip;
            
            l_hideButtonRect.x = l_buttonWidth/2;
            GUI.Button(l_hideButtonRect, l_infoContent, l_infoButtonStyle);                   
        }
        
        // SLIDER PROPERTIES
        Rect l_sliderBoxRect = new Rect(0, l_sceneRect.height - (l_buttonHeight  * 1.9f), l_buttonWidth * 5, l_buttonHeight * .8f);
        GUI.Label(l_sliderBoxRect, "", BoxStyle());
        
        // DISPLAY INFO
        GUIStyle l_centeredText = new GUIStyle();
        l_centeredText.alignment = TextAnchor.LowerCenter;
        
        l_centeredText.normal.textColor = Color.black;
        
        if(HasIntensityValue())
            GUI.Label(l_sliderBoxRect, "- Severity +", l_centeredText);

        if(HasIntensityValue())
            l_centeredText.alignment = TextAnchor.UpperCenter;
        else
            l_centeredText.alignment = TextAnchor.MiddleCenter;
        GUI.Label(l_sliderBoxRect, GetInfoDisplay(), l_centeredText);
        
        float l_sliderPadding = l_buttonWidth / 3f;
        float l_knobSize = 12;
        Rect l_sliderRect =  new Rect(l_sliderPadding, l_sceneRect.height - (l_buttonHeight * 1 +  l_buttonHeight * 0.9f/2 + l_knobSize/2),
                                         l_buttonWidth * 4, l_knobSize);

        // SLIDER STYLE
        GUIStyle l_railStyle = new GUIStyle();
        l_railStyle.normal.background = m_sliderRail;
        l_railStyle.fixedWidth = l_sliderRect.width;
        l_railStyle.fixedHeight = l_knobSize;
        GUIStyle l_knobStyle = new GUIStyle();
        l_knobStyle.fixedWidth = l_knobSize;
        l_knobStyle.fixedHeight = l_knobSize;
        l_knobStyle.normal.background = m_buttonNormal;

        // SLIDER VALUE
        float l_lastValue = m_severityValue;
        
        if(HasIntensityValue()) 
            m_severityValue = GUI.HorizontalSlider(l_sliderRect, m_severityValue, 0, 1, l_railStyle, l_knobStyle);

        if(l_lastValue != m_severityValue)
            UpdateFilterIntensity();

        // BUTTONS
        int l_buttonNumber = 0;

        // NORMAL VISION
        Rect l_buttonRect = new Rect(0, l_sceneRect.height - l_buttonHeight, l_buttonWidth, l_buttonHeight);
        if(GUI.Button(l_buttonRect, "N", ButtonStyle(l_buttonNumber)))
            SetFilter(l_buttonNumber);

        GUI.DrawTexture(l_buttonRect, m_iconNormal, ScaleMode.ScaleToFit);
        
        // PROTANOPIA
        l_buttonNumber++;
        l_buttonRect = new Rect(l_buttonWidth * l_buttonNumber, l_sceneRect.height - l_buttonHeight, l_buttonWidth, l_buttonHeight);
        if(GUI.Button(l_buttonRect, "P", ButtonStyle(l_buttonNumber)))
            SetFilter(l_buttonNumber);
        
        GUI.DrawTexture(l_buttonRect, m_iconProtanopia, ScaleMode.ScaleToFit);
        
        // DEUTERANOPIA
        l_buttonNumber++;
        l_buttonRect.x = l_buttonWidth * l_buttonNumber;
        if(GUI.Button(l_buttonRect, "D", ButtonStyle(l_buttonNumber)))
            SetFilter(l_buttonNumber);
        
        GUI.DrawTexture(l_buttonRect, m_iconDeuteranopia, ScaleMode.ScaleToFit);
        
        // TRITANOPIA
        l_buttonNumber++;
        l_buttonRect.x = l_buttonWidth * l_buttonNumber;
        if(GUI.Button(l_buttonRect, "T", ButtonStyle(l_buttonNumber)))
            SetFilter(l_buttonNumber);

        GUI.DrawTexture(l_buttonRect, m_iconTritanopia, ScaleMode.ScaleToFit);
        
        // ACHROMATOPSIA
        l_buttonNumber++;
        l_buttonRect.x = l_buttonWidth * l_buttonNumber;

        if(GUI.Button(l_buttonRect, "A", ButtonStyle(l_buttonNumber)))
            SetFilter(l_buttonNumber);

        GUI.DrawTexture(l_buttonRect, m_iconAchromatopsia, ScaleMode.ScaleToFit);

        Handles.EndGUI();
    }

    private static void SetFilter(int _type){
        DisableAllKeywords();
        m_selectedButton = _type;
        m_filterMaterial.EnableKeyword(GetKeywordByType(_type));
        m_severityValue = 1;
        UpdateFilterIntensity();
    }

    private static void UpdateFilterIntensity() => m_filterMaterial.SetFloat(m_intensityProperty, m_severityValue);

    private static string GetKeywordByType(int _type){
        switch(_type){
            case 0:
                return "_CVTYPE_TRICHROMACY";
            case 1:
                return "_CVTYPE_PROTANOPIA";
            case 2:
                return "_CVTYPE_DEUTERANOPIA";
            case 3:
                return "_CVTYPE_TRITANOPIA";
            case 4:
                return "_CVTYPE_ACHROMATOPSIA";
        }
        return null;
    }

    private static void DisableAllKeywords(){
        for(int i = 0; i < 5; i++){
            m_filterMaterial.DisableKeyword(GetKeywordByType(i));
        }
    }

    static GUIStyle ButtonStyle(int _buttonIndex){
        GUIStyle l_style = new GUIStyle();
        
        // Buttons
        l_style.normal.background = _buttonIndex == m_selectedButton ? m_buttonSelected : m_buttonNormal;
        l_style.hover.background = _buttonIndex == m_selectedButton ? m_buttonSelected : m_buttonHover;
        
        // Text
        l_style.fontSize = 18;
        l_style.alignment = TextAnchor.MiddleCenter;
        l_style.fontStyle =  _buttonIndex == m_selectedButton ? FontStyle.Bold : FontStyle.Normal;
        l_style.normal.textColor = _buttonIndex == m_selectedButton ? Color.black : Color.white;
        l_style.hover.textColor = _buttonIndex == m_selectedButton ? Color.black : Color.white;

        return l_style;
    }

    static GUIStyle BoxStyle(){
        GUIStyle l_style = new GUIStyle();
        
        // Buttons
        l_style.normal.background = m_boxTexture;
        l_style.normal.textColor = Color.black;

        return l_style;
    }

    static string GetInfoDisplay(){
        if(m_severityValue != 1){
            string l_type = "";
            switch(m_selectedButton){
                case 0:
                    return "Normal Vision / Trichromacy";
                case 1:
                    l_type = "Protanomaly";
                    break;
                case 2:
                    l_type = "Deuteranomaly";
                    break;
                case 3:
                    l_type = "Tritanomaly";
                    break;
                case 4:
                    l_type = "Achromatopsia";
                    break;
            }
            return l_type + " " + Mathf.Round(m_severityValue * 100) + "%";
        }
        else{
            switch(m_selectedButton){
                case 0:
                    return "Normal Vision / Trichromacy";
                case 1:
                    return "Protanopia";
                case 2:
                    return "Deuteranopia";
                case 3:
                    return "Tritanopia";
                case 4:
                    return "Achromatopsia";
            }
        }
        return null;
    }

    static bool HasIntensityValue(){
        if(m_selectedButton == 0 || m_selectedButton == 4) return false;
        return true;
    }

    static void SetComponentLastInCamera(){
        if(Camera.main == null) return;
        
        ApplyFilter[] l_currentFilters = Camera.main.GetComponents<ApplyFilter>();
        foreach(ApplyFilter f in l_currentFilters){
            if(m_filterMaterial == f.FilterMaterial && (f.FilterTag == m_tag || f.FilterTag == null || f.FilterTag == "")) {
                DestroyImmediate(f);
            }
        }

        if(m_filterMaterial == null) return;

        ApplyFilter l_filterComponent = Camera.main.gameObject.AddComponent<ApplyFilter>();
        l_filterComponent.SetMaterial(m_filterMaterial, m_tag);  
        l_filterComponent.hideFlags = HideFlags.HideInInspector; 
    }
}
#endif