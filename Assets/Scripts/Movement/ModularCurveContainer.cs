using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModularCurveContainer : ScriptableObject
{
    [SerializeField] private List<ModularCurveBaseHolder> modularCurveBaseHolders;

    public void CalculateAdjustmentCoefficients()
    {
        foreach (ModularCurveBaseHolder modularCurveBaseHolder in modularCurveBaseHolders)
        {
            modularCurveBaseHolder.CalculateAdjustmentCoefficient();
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


