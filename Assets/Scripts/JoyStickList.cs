using UnityEngine;
using System;

public class JoystickInput : MonoBehaviour
{
    public Joystick joyStickList;
    public event Action OnPressedUp; // event

    private bool wasPressed = false;

    void Update()
    {
        if (joyStickList.Vertical > 0.7f)
        {
            if (!wasPressed)
            {
                OnPressedUp?.Invoke(); // fire event
                wasPressed = true;
            }
        }
        else
        {
            wasPressed = false;
        }
    }
}
