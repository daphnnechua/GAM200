using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FullScreenSwitch : MonoBehaviour
{
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.F11))
        {
            SwitchToFullScreen();
        }
    }

    public void SwitchToFullScreen()
    {
        if (Screen.fullScreen)
        {
            Screen.fullScreen = false;
            Screen.SetResolution(1920, 1080, false); 
        }
        else
        {
            Screen.fullScreen = true;
        }
    }
}
