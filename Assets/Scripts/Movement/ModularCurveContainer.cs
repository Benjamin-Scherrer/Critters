using System.Collections.Generic;
using UnityEngine;

public class ModularCurveContainer : ScriptableObject
{
    [SerializeField] private List<ModularCurveBaseHolder> modularCurveBaseHolders;
    [SerializeField] private float averageForce = 1;
    public void CalculateAdjustmentCoefficients()
    {
        //Get total weight
        float weightSum = 0;
        foreach(ModularCurveBaseHolder modularCurveBaseHolder in modularCurveBaseHolders)
        {
            weightSum += modularCurveBaseHolder.GetWeight;
        }

        //Calculate force coefficient
        float forceCoef = averageForce / weightSum;

        //Calculate adjustment coefficients
        foreach (ModularCurveBaseHolder modularCurveBaseHolder in modularCurveBaseHolders)
        {
            modularCurveBaseHolder.CalculateAdjustmentCoefficient(forceCoef);
        }
    }

    public float Evaluate(float time)
    {
        float value = 0;

        foreach(ModularCurveBaseHolder modularCurveBaseHolder in modularCurveBaseHolders)
        {
            value += modularCurveBaseHolder.Evaluate(time);
        }

        return value;
    }
}


