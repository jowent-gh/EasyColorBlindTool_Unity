using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetHighContrastColorByTag : MonoBehaviour {
    [SerializeField]
    string m_highContrastTag;

    Renderer m_renderer;
    MaterialPropertyBlock m_mpb;

    private void Start() {
        HighContrastController l_hcInstance = HighContrastController.m_instance;
        if(l_hcInstance == null){
            Debug.LogWarning("There is not HighContrastController component on the Scene, High Contrast Color will not be applied");
            return;
        }
        
        HighContrastTagColors l_hcColors = l_hcInstance.m_colorSetup;

        if(l_hcColors != null){
            Color l_overrideColor = l_hcColors.GetColor(m_highContrastTag);
            m_renderer = GetComponent<Renderer>();
            m_mpb = new MaterialPropertyBlock();
            m_mpb.SetColor("_HighContrastColor", l_overrideColor);
            
            m_renderer.SetPropertyBlock(m_mpb); 
        }
    }    
}
