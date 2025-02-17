using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModularCurveBase : ScriptableObject
{
    [Header("Curve")]
    [SerializeField] private AnimationCurve valueCurve = AnimationCurve.Constant(0, 1, 1);

    public AnimationCurve GetValueCurve { get => valueCurve; }
}
