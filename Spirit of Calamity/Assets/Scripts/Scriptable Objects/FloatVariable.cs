using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Float Variable")]
public class FloatVariable : ScriptableObject, ISerializationCallbackReceiver
{
    public float value;

    [System.NonSerialized] public float runtimeValue; // Value that reloads with "value" at the start of scene 

    public void OnBeforeSerialize() { }

    public void OnAfterDeserialize()
    {
        runtimeValue = value; // runtimeValue = value at reload
    }

    public void SubtractValue(float subtrahend) // Subtract value (Warning: subtracted value will not reload
    {                                               // to initial, please use the runtime value to preserve initial value
        value -= subtrahend;
    }

    public void SubtractRuntimeValue(float subtrahend) // Subtract from runtime value; a value that will reload
    {
        runtimeValue -= subtrahend;
    }
}
