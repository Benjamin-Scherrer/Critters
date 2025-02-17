using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModularCurveContainer : ScriptableObject
{
   [Serializable]
   private class ModularCurveBaseHolder
    {
        [SerializeField] ModularCurveBase modularCurveBase;
        [SerializeField] private float valueCycleDuration = 1;
        [SerializeField] private float valueMultiplier = 1;
        [SerializeField] private float offset = 0;

        public float Evaluate(float time)
        {
            AnimationCurve valueCurve = modularCurveBase.GetValueCurve;
            float curveDuration = valueCurve[valueCurve.length - 1].time;
            float timeMultiplier = curveDuration / valueCycleDuration;
            float localTime = ((time+offset) % valueCycleDuration) * timeMultiplier;
            float value = valueCurve.Evaluate(localTime);
            return value * valueMultiplier;

        }
    }

    [SerializeField] private List<ModularCurveBaseHolder> valueCurves;

    public float Evaluate(float time)
    {
        float value = 0;

        foreach(ModularCurveBaseHolder modularCurveBaseHolder in valueCurves)
        {
            value += modularCurveBaseHolder.Evaluate(time);
        }

        return value;
    }
}


