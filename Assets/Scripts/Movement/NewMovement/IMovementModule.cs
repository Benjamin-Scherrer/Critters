using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IMovementModule
{
    public ModularCurveContainer GetForceCurveContainer { get; set; }
    public float GetForceMultiplier { get; set; }

    public abstract void Run(Rigidbody rigidbody, Transform forceTarget, float forceAllocated, float timeOffset);
}
