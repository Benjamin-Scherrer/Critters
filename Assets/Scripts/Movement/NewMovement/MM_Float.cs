using System;
using UnityEngine;

[Serializable]
public struct MM_Float : IMovementModule
{
    private ModularCurveContainer forceCurveContainer;
    private float forceMultiplier;
    public ModularCurveContainer GetForceCurveContainer { get => forceCurveContainer; set => forceCurveContainer = value; }
    public float GetForceMultiplier { get => forceMultiplier; set => forceMultiplier = value; }

    public void Run(Rigidbody rigidbody, Vector3 GetForceTargetPosition, float forceAllocated, float timeOffset)
    {
        float fluxuationOverTime = forceCurveContainer.Evaluate(Time.time + timeOffset);

        float floorY = 0;
        float idealHeight = 6;
        float maxDistanceFromIdealHeight = 4;

        float distanceFromIdealHeight = GetForceTargetPosition.y - floorY - idealHeight;
        float heightScale = Mathf.Clamp(1 - (distanceFromIdealHeight / maxDistanceFromIdealHeight), -1, 1);

        rigidbody.AddForceAtPosition(fluxuationOverTime * forceAllocated * GetForceMultiplier * heightScale * new Vector3(0, 1, 0).normalized, GetForceTargetPosition, ForceMode.Impulse);
    }
}