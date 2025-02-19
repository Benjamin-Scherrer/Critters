using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ModularCurveBaseHolder
{
    [SerializeField] ModularCurveBase modularCurveBase;
    [SerializeField] private float valueCycleDuration = 1;
    [SerializeField] private float weight = 1;
    [SerializeField] private float averageForce = 1;
    [SerializeField] private float adjustmentCoefficient = 1;
    [SerializeField] private float offset = 0;

    public float GetWeight { get => weight; }

    //PUBLIC
    public void CalculateAdjustmentCoefficient(float forceCoef)
    {
        //Set neww average force
        averageForce = forceCoef * weight;

        //Calculate adjustment coefficient
        float currentAverage = CalculateCurrentAverage();

        //prevent divide by zero
        if(Mathf.Abs(currentAverage) < 0.001) 
        {
            adjustmentCoefficient = 0;
                return; 
        }

        adjustmentCoefficient = averageForce / currentAverage;
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

    //PRIVATE
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
}

