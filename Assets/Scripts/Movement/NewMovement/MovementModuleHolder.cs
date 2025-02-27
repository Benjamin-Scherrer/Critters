using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class MovementModuleHolder
{
    public enum MovementModuleType
    {
        Floating,
        Snake,
        Waypoint
    }

    [SerializeField] private MovementModuleType movementModuleType;
    [SerializeField] private ModularCurveContainer modularCurveContainer;
    [SerializeField] private float Weight = 1;

    private float forceMultiplier = 1;

    public float GetWeight { get => Weight; }
    public float SetForceMultiplier { set => forceMultiplier = value; }

    //Make special editor for selection later

    public IMovementModule GetMovementModule
    {
        get
        {
            switch (movementModuleType)
            {
                case MovementModuleType.Floating:
                    var mm_Basic = new MM_Float
                    {
                        GetForceCurveContainer = modularCurveContainer,
                        GetForceMultiplier = forceMultiplier
                    };
                    return mm_Basic;
                case MovementModuleType.Snake:
                    var MM_Snake = new MM_Snake
                    {
                        GetForceCurveContainer = modularCurveContainer,
                        GetForceMultiplier = forceMultiplier
                    };
                    return MM_Snake;
                case MovementModuleType.Waypoint:
                    var MM_Waypoint = new MM_Waypoint
                    {
                        GetForceCurveContainer = modularCurveContainer,
                        GetForceMultiplier = forceMultiplier
                    };
                    return MM_Waypoint;
                default:
                    throw new NotImplementedException("Movement Module Type: " + movementModuleType.ToString());
            }
        }
    }
}


