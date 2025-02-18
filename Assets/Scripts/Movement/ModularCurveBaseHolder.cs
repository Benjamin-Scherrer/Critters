using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ModularCurveBaseHolder
{
    [SerializeField] ModularCurveBase modularCurveBase;
    [SerializeField] private float valueCycleDuration = 1;
    [SerializeField] private float averageValue = 1;
    [SerializeField] private float adjustmentCoefficient = 1;
    [SerializeField] private float offset = 0;

    private float CalculateCurrentAverage()
    {
        AnimationCurve valueCurve = modularCurveBase.GetValueCurve;
        float curveDuration = valueCurve[valueCurve.length - 1].time;
        float sum = 0;
        int sampleCount = 100; // Number of samples to approximate the average

        for (int i = 0; i <= sampleCount; i++)
        {
            float time = (i / (float)sampleCount) * curveDuration;
            sum += valueCurve.Evaluate(time);
        }

        return sum / (sampleCount + 1);
    }

    public void CalculateAdjustmentCoefficient()
    {
        float currentAverage = CalculateCurrentAverage();
        if(Mathf.Abs(currentAverage) < 0.001) 
        {
            adjustmentCoefficient = 0;
                return; 
        }

        adjustmentCoefficient = averageValue / currentAverage;
    }

    public float Evaluate(float time)
    {
        AnimationCurve valueCurve = modularCurveBase.GetValueCurve;
        float curveDuration = valueCurve[valueCurve.length - 1].time;
        float timeMultiplier = curveDuration / valueCycleDuration;
        float localTime = ((time + offset) % valueCycleDuration) * timeMultiplier;
        float value = valueCurve.Evaluate(localTime);
        return value * adjustmentCoefficient;

    }
}

