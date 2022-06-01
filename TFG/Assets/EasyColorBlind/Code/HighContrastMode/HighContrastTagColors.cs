using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class TagColor{
    public string tag;
    public Color color;
}

[CreateAssetMenu(fileName = "High Contrast Tag Colors", menuName = "Easy Color Blind/High Contrast Tag Colors", order = 1)]
public class HighContrastTagColors : ScriptableObject {
    public List<TagColor> m_tagList;


    public Color GetColor(string _tag){
        foreach(TagColor t in m_tagList){
            if(t.tag == _tag)
                return t.color;
        }
        return Color.black;
    }
}
