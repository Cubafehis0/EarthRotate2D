using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchControlMode : MonoBehaviour
{
    public void ClickToSwitchMode()
    {
        RotateControl rotateControl = RotateControl.rotateControlInstance;
        if(rotateControl.controlMode==ControlMode.Emergency)
        {
            rotateControl.controlMode = ControlMode.Normal;
            rotateControl.scale = 5;
            
        }
        else if (rotateControl.controlMode == ControlMode.Normal)
        {
            rotateControl.scale = 2;
            rotateControl.controlMode = ControlMode.Emergency;
        }
        rotateControl.isSwitching = true;
    }
}
