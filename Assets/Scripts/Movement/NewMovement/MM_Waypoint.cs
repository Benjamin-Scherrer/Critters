using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct MM_Waypoint : IMovementModule
{
    private ModularCurveContainer forceCurveContainer;
    private float forceMultiplier;
    public ModularCurveContainer GetForceCurveContainer { get => forceCurveContainer; set => forceCurveContainer = value; }
    public float GetForceMultiplier { get => forceMultiplier; set => forceMultiplier = value; }


    //Custom Variables
    private Transform waypoint;

    private Transform GetWaypoint
    {
        get
        {
            if (waypoint == null)
            {
                waypoint = GameObject.Find("Waypoint").transform;
            }
            return waypoint;
        }
    }

    public void Run(Rigidbody rigidbody, Transform forceTarget, float forceAllocated, float timeOffset)
    {
        float fluxuationOverTime = forceCurveContainer.Evaluate(Time.time + timeOffset);

        Vector3 forceWorldDirection = (GetWaypoint.position - forceTarget.position);
        forceWorldDirection.y = Mathf.Max(forceWorldDirection.y, 0.1f);
        forceWorldDirection.Normalize();

        //Debug.Log("Force Final: " + fluxuationOverTime * forceAllocated * GetForceMultiplier * forceWorldDirection);
        rigidbody.AddForceAtPosition(fluxuationOverTime * forceAllocated * GetForceMultiplier * forceWorldDirection, forceTarget.position, ForceMode.Impulse);


        //Debug.DrawLine(forceTarget.position, GetWaypoint.position, Color.red);  
        //Debug.DrawLine(forceTarget.position, forceTarget.position + forceWorldDirection * 2f, Color.green);

        if (Vector3.Distance(forceTarget.position, GetWaypoint.position) < 2f) //fixed variable bc fuck it.
        {
            waypoint = WayPointManager.Instance.GetWayPoint(GetWaypoint);
        }
    }
}
