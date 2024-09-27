using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class LinearAttenuation : AttunationFunction
{
    public float linearCoefficient = 0.4f;
    public override float ApplyAttenuationFunction(float baseValue,float distanceRatio)
    {
        return baseValue - (linearCoefficient * (distanceRatio));
    }
}
