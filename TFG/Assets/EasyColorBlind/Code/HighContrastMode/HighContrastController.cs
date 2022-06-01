using UnityEngine;
using System.Collections;
using UnityEditor;

[ExecuteAlways]
public class HighContrastController : MonoBehaviour {
    public static HighContrastController m_instance;    
    
    [Header("Recoloring")]
    [Tooltip("If not NULL will override colors by tag setup")]
    public HighContrastTagColors m_colorSetup;    
    
    [Range(0, 1)] public float m_highContrast = 0;
    
    [SerializeField]
    [Range(0.01f, 100)] float m_DistanceStart = .01f; 
    
    [SerializeField]
    [Range(0.01f, 100)] float m_DistanceEnd = 20;
    
    [SerializeField]
    [Range(0, 1)] float m_depthIntensity = .4f;
    
    [SerializeField]
    [Range(0, 1)] float m_albedoIntensity = .15f;
    
    [Space]
    [Header("Outline")]
    [SerializeField]
    bool m_addOutline = true;
    
    [SerializeField]
    [Range(0, 1)] float m_depthThreshold = .2f;
    
    [SerializeField]
    [Range(0, 3)] float m_normalThreshold = 1.5f;
    
    [SerializeField]
    [Range(0, 1)] float m_outlineAlpha = .2f;
    
    [SerializeField]
    [Range(1, 5)] float m_outlineThickness = 2;

    Material m_outlineMaterial;
    const string m_outlineFilterTag = "HighContrastOutline";

    private void Awake() {
        if(m_instance == null)
            m_instance = this;
        else{
            Destroy(gameObject);
        }
    }

    private void OnEnable() {
        Camera.main.depthTextureMode = DepthTextureMode.DepthNormals;
        m_outlineMaterial = Resources.Load<Material>("EasyColorBlind/mat_outlineHC");

        ApplyFilter[] l_cameraFilters = Camera.main.GetComponents<ApplyFilter>();
        foreach(ApplyFilter f in l_cameraFilters){ 
            if(m_outlineMaterial == f.FilterMaterial && (f.FilterTag == m_outlineFilterTag || f.FilterTag == null || f.FilterTag == "")) {
                DestroyImmediate(f);
            }
        } 

        if(m_outlineMaterial == null) return;

        ApplyFilter l_outlineFilter = Camera.main.gameObject.AddComponent<ApplyFilter>();
        l_outlineFilter.SetMaterial(m_outlineMaterial, m_outlineFilterTag);
        l_outlineFilter.hideFlags = HideFlags.HideInInspector;
    }

    private void Update() {
        Shader.SetGlobalFloat("_IsHighContrast", m_highContrast);
        Shader.SetGlobalFloat("_HCZStart", m_DistanceStart);
        Shader.SetGlobalFloat("_HCZEnd", m_DistanceEnd);
        Shader.SetGlobalFloat("_CamNear", Camera.main.nearClipPlane);
        Shader.SetGlobalFloat("_CamFar", Camera.main.farClipPlane);
        Shader.SetGlobalFloat("_HCZIntensity", m_depthIntensity);
        Shader.SetGlobalFloat("_HCAlbedo", m_albedoIntensity);

        if(m_DistanceStart > m_DistanceEnd)
            m_DistanceEnd = m_DistanceStart;
        
        if(m_outlineMaterial == null) return;

        m_outlineMaterial.SetFloat("_HasOutline", m_addOutline ? 1 : 0);

        m_outlineMaterial.SetFloat("_DepthThreshold", m_depthThreshold);
        m_outlineMaterial.SetFloat("_NormalThreshold", m_normalThreshold);
        m_outlineMaterial.SetFloat("_OutlineAlpha", m_outlineAlpha);
        m_outlineMaterial.SetFloat("_Thickness", m_outlineThickness);
    }

    private void OnDisable() {
        Shader.SetGlobalFloat("_IsHighContrast", 0);
    }
}
