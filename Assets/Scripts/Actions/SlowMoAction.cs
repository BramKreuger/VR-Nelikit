using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class SlowmoAction : MonoBehaviour
{
    public InputActionReference slomoReference = null;
    public PlayPhaseManager playPhase;

    private void Update()
    {
        float value = slomoReference.action.ReadValue<float>();
        ChangeSlomo(value);
    }

    void ChangeSlomo(float value)
    {
        //Get raw value. Cap the lower boundry at 1 -> 100% speed. And 10 -> 10% speed
        //value = Math.Clamp(value, 1f, 10f);
        //playPhase.slowFactor = value;
    }
}
