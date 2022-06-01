using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TCrosshairType{
    LOCKED, MOUSE
}

public class Crosshair : MonoBehaviour {
    [SerializeField]
    TCrosshairType m_type = TCrosshairType.MOUSE;

    private void Update() {
        switch(m_type){
            case TCrosshairType.MOUSE:
                Vector3 l_mousePos = Input.mousePosition;
                l_mousePos.x = Mathf.Clamp(l_mousePos.x, 1, Screen.width-1);
                l_mousePos.y = Mathf.Clamp(l_mousePos.y, 1, Screen.height-1);
                transform.position = l_mousePos;
                break;
            case TCrosshairType.LOCKED:
                transform.position = new Vector3(Screen.width/2, Screen.height/2, 0);
                break;
        }
    }
}
