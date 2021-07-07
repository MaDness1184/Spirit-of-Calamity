using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization; // used to serialize the class

[System.Serializable] // Make the class serializable so we can see it's variable's in the inspector
public class FloatReference
{
    public bool useConstant = false;
    public float constantValue;
    public FloatVariable variable;

    public float GetValue()
    {
        return useConstant ? constantValue : variable.value;
    }

    public float GetRuntimeValue()
    {
        if (useConstant == false)
        {
            return variable.runtimeValue;
        }
        else
        {
            Debug.Log("FloatReference: Cannot return runtimeValue of constant type (returned constant value)");
            return constantValue;
        }
    }

    public void SubtractValue(float subtrahend) // Subtract from Constant or FV value
    {
        if (useConstant == true)
        {
            constantValue -= subtrahend;
        }
        else
        {
            variable.SubtractValue(subtrahend);
        }
    }

    public void SubtractRuntimeValue(float subtrahend) // Subtract from RTFV value
    {
        if (useConstant == false)
        {
            variable.SubtractRuntimeValue(subtrahend);
        }
        else
        {
            Debug.Log("FloatReference: Cannot subtract runtimeValue of constant type"); // Error if passing constant value
        }
    }
}
