using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IMovementModule
{
    public ModularCurveContainer GetForceCurveContainer { get; set; }
    public float GetForceMultiplier { get; set; }

    public abstract void Run(Rigidbody rigidbody, Vector3 GetForceTargetPosition, float forceAllocated, float timeOffset);
}
