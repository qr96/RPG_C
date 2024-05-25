using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager
{
    bool useJoystick = false;

    public float GetInputHorizontal()
    {
        if (useJoystick)
            return UIManager.Instance.GetInputHorizontal();
        else
            return Input.GetAxis("Horizontal");
    }

    public float GetInputVertical()
    {
        if (useJoystick)
            return UIManager.Instance.GetInputVertical();
        else
            return Input.GetAxis("Vertical");
    }
}
