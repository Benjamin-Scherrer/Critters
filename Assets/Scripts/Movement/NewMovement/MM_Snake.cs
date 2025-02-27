using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct MM_Snake : IMovementModule
{
    private ModularCurveContainer forceCurveContainer;
    private float forceMultiplier;
    public ModularCurveContainer GetForceCurveContainer { get => forceCurveContainer; set => forceCurveContainer = value; }
    public float GetForceMultiplier { get => forceMultiplier; set => forceMultiplier = value; }


    //Custom Variables
    private Transform neighbourReference;
    private bool isMainHead;
    public void SetSnakeMainHeadLocalDirection(Transform neighbourReference)
    {
       this.neighbourReference = neighbourReference;
        isMainHead = true;
    }

    public void SetSnakeSideHeadLocalDirection(Transform neighbourReference)
    {
        this.neighbourReference = neighbourReference;
        isMainHead = false;
    }


    public void Run(Rigidbody rigidbody, Transform forceTarget, float forceAllocated, float timeOffset)
    {
        float fluxuationOverTime = forceCurveContainer.Evaluate(Time.time + timeOffset);

        Vector3 forceWorldDirection = isMainHead ? (forceTarget.position - neighbourReference.position) : (neighbourReference.position - forceTarget.position);
        forceWorldDirection.y = Mathf.Max(forceWorldDirection.y, 0.1f);
        forceWorldDirection.Normalize();

        //Debug.Log("Force Final: " + fluxuationOverTime * forceAllocated * GetForceMultiplier * forceWorldDirection);
        rigidbody.AddForceAtPosition(fluxuationOverTime * forceAllocated * GetForceMultiplier * forceWorldDirection, forceTarget.position, ForceMode.Impulse);
    }
}
