using UnityEngine;
using UnityEditor;
using UnityEngine.UI;

#if UNITY_EDITOR
public class ColorBlindElementSetup : Editor {
    
    [MenuItem("Easy Color Blind/High Contrast Mode/Create Configuration")]
    public static void SetupHighContrast(){
        string l_goName = "High Contrast Config"; 
        
        GameObject l_empty = new GameObject("");
        GameObject l_go = Instantiate(l_empty);
        
        l_go.name = l_goName;
        l_go.AddComponent<HighContrastController>();
        
        DestroyImmediate(l_empty);
    }


    [MenuItem("Easy Color Blind/Visual Color Code/Create Color Reader")]
    public static void SetupColorReader(){
        GameObject l_empty = new GameObject("");
                
        GameObject l_goCanvas = Instantiate(l_empty);
        l_goCanvas.name = "Canvas VCC";;
        l_goCanvas.layer = 5; // UI Layer
        Canvas l_canvas = l_goCanvas.AddComponent<Canvas>();
        l_canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        l_goCanvas.AddComponent<CanvasScaler>();
        
        GameObject l_goColorReader = Instantiate(l_empty);
        l_goColorReader.name = "Color Reader";
        l_goColorReader.layer = 5; // UI Layer
        ColorReader l_colorReader = l_goColorReader.AddComponent<ColorReader>();
        l_goColorReader.transform.SetParent(l_goCanvas.transform);
        l_goColorReader.transform.localPosition = Vector3.zero;

        // Position To Read
        GameObject l_target = Instantiate(l_empty);
        l_target.name = "VCC TARGET";
        l_target.layer = 5; // UI Layer
        l_target.transform.SetParent(l_goColorReader.transform);
        l_target.transform.localPosition = Vector3.zero;
        Image l_imageTarget = l_target.AddComponent<Image>();
        l_target.AddComponent<Crosshair>();
        
        // Figure
        GameObject l_figure = Instantiate(l_empty);
        l_figure.name = "Figure";
        l_figure.layer = 5; // UI Layer
        l_figure.transform.SetParent(l_goColorReader.transform);
        l_figure.transform.localPosition = Vector3.zero;
        Image l_imageBackground = l_figure.AddComponent<Image>();
        
        GameObject l_Red = Instantiate(l_empty);
        l_Red.name = "RED";
        l_Red.layer = 5; // UI Layer
        l_Red.transform.SetParent(l_figure.transform);
        l_Red.transform.localPosition = Vector3.zero;
        Image l_imageRed = l_Red.AddComponent<Image>();
        
        GameObject l_Green = Instantiate(l_empty);
        l_Green.name = "GREEN";
        l_Green.layer = 5; // UI Layer
        l_Green.transform.SetParent(l_figure.transform);
        l_Green.transform.localPosition = Vector3.zero;
        Image l_imageGreen = l_Green.AddComponent<Image>();
        
        GameObject l_Blue = Instantiate(l_empty);
        l_Blue.name = "BLUE";
        l_Blue.layer = 5; // UI Layer
        l_Blue.transform.SetParent(l_figure.transform);
        l_Blue.transform.localPosition = Vector3.zero;
        Image l_imageBlue = l_Blue.AddComponent<Image>();
        
        GameObject l_sv = Instantiate(l_empty);
        l_sv.name = "LIGHT/DARK";
        l_sv.layer = 5; // UI Layer
        l_sv.transform.SetParent(l_figure.transform);
        l_sv.transform.localPosition = Vector3.zero;
        Image l_imageSV = l_sv.AddComponent<Image>();

        l_colorReader.SetComponents(l_imageTarget, l_imageBackground, l_imageRed, l_imageGreen, l_imageBlue, l_imageSV);

        DestroyImmediate(l_empty);
    }

    [MenuItem("Easy Color Blind/Visual Color Code/Create Figure Setup")]
    public static void SetupVisualCodeColorTexture(){
        string l_goName = "Visual Color Code Config"; 
        
        GameObject l_empty = new GameObject("");
        GameObject l_go = Instantiate(l_empty);
        
        l_go.name = l_goName;
        ColorFigureConfiguration l_cfc = l_go.AddComponent<ColorFigureConfiguration>();
        ColorFigureTextures l_sample = Resources.Load<ColorFigureTextures>("EasyColorBlind/sample_VCCTextures");
        if(l_sample != null)
            l_cfc.m_textures = l_sample;

        
        DestroyImmediate(l_empty);
    }
    
}
#endif
