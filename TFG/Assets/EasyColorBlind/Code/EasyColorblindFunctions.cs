using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct ColorFigure{
    public float red;
    public float green;
    public float blue;
    public int sv;
}

public class EasyColorblindFuncions {
    
    public static ColorFigure ColorToFigure(Color _color, float _thresholdSV = .75f, float _thersholdBW = .25f){
        
        Color.RGBToHSV(_color, out float l_colH, out float l_colS, out float l_colV);
        
        int l_separations = 6;
        float l_closestKey = 0;

        for(int i = 0; i <= l_separations; i++){
            // Find closest posterized hue;
            if(Mathf.Abs(l_colH - (1.0f/l_separations * i)) < 1.0f/(l_separations * 2)){
                l_closestKey = i;
                continue;
            }
        }
        
        float l_r = 0;
        float l_g = 0;
        float l_b = 0;
        int l_sv = 0; // 0 normal, 1 - Light, 2 - Dark

        // Set RGB Figure Values
        if(l_closestKey == 0 || l_closestKey == l_separations){ 
            l_r = 1;
            l_g = 0;
            l_b = 0;
        }
        else{
            switch(l_closestKey){
                case 1: // Yellow
                    l_r = 1;
                    l_g = 1;
                    l_b = 0;
                    break;
                case 2: // Green
                    l_r = 0;
                    l_g = 1;
                    l_b = 0;
                    break;
                case 3: // Cyan
                    l_r = 0;
                    l_g = 1;
                    l_b = 1;
                    break;
                case 4: // Blue
                    l_r = 0;
                    l_g = 0;
                    l_b = 1;
                    break;
                case 5: // Pink
                    l_r = 1;
                    l_g = 0;
                    l_b = 1;
                    break;
            }
        }

        // Brighter-Darker
        if(l_colS < l_colV && l_colS < _thresholdSV) // brighter
            l_sv = 1;
        else if(l_colV < l_colS && l_colV < _thresholdSV) // darker
            l_sv = 2;     
                
        // Special Cases: 0 Saturation / Full Value
        if(l_colV < _thersholdBW){ // Black
            l_sv = 2;
            l_r = 0;
            l_g = 0;
            l_b = 0;
        }
        else if(l_colS < _thersholdBW){ // White
            l_sv = 1;
            l_r = 1;
            l_g = 1;
            l_b = 1;
        }

        ColorFigure l_cf = new ColorFigure();
        l_cf.red = l_r;
        l_cf.green = l_g;
        l_cf.blue = l_b;
        l_cf.sv = l_sv;

        return l_cf;
    } 
}
