using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class AttunationFunction : ScriptableObject
{
    public virtual float ApplyAttenuationFunction(float baseValue, float distanceRatio)
    {
        return 0.0f;
    }
}
