using UnityEngine;
using UnityEditor;

[ExecuteInEditMode]
[ImageEffectAllowedInSceneView]
public class ApplyFilter : MonoBehaviour {
    
    [SerializeField] Material m_filterMaterial;

    string m_tag = "";
    RenderTexture m_rt;
    bool m_render;

    public string FilterTag => m_tag;
    public Material FilterMaterial => m_filterMaterial;

    // #if UNITY_EDITOR
    private void OnRenderImage(RenderTexture src, RenderTexture dest) {
        Graphics.Blit(src, dest, m_filterMaterial);
        
        m_render = Camera.main.GetComponents<ApplyFilter>()[0] == this;
        if(m_render){
            if(m_rt == null)
                m_rt = new RenderTexture(Screen.width, Screen.height, 0);        
            
            Graphics.Blit(src, m_rt);
        }
    }
    // #endif

    public void SetMaterial(Material _mat, string _tag){
        m_filterMaterial = _mat; 
        m_tag = _tag;
    }

    public RenderTexture GetUnfilteredTexture(){
        if(m_render == false) return null;   
        return m_rt; 
    }
}
