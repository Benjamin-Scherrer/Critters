using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public abstract class Movement_Abstract : MonoBehaviour
{
    [Header("General")]
    [SerializeField] protected MovementType movementType = MovementType.MainMovement;

    [Header("Force")]
    [SerializeField] protected ModularCurveContainer forceCurveContainer;
    
    //[Header("Velocity")]
    //[SerializeField] protected float maxLinearVelocity = 10;
    //[SerializeField] protected float maxAngularVelocity = 10;

    [Header("Direction")]
    [SerializeField] protected bool localDirection = false;
    [SerializeField] protected ModularCurveContainer directionXCurveContainer;
    [SerializeField] protected ModularCurveContainer directionYCurveContainer;
    [SerializeField] protected ModularCurveContainer directionZCurveContainer;

    [Header("Debug")]
    [SerializeField][Range(0, 10)] protected float GizmoSize = 1;

    //References
    protected Rigidbody rb;

    //Extra Variables
    private float timeOffset = 0;
    private float extraForceMultiplier = 1;
    public MovementType MovementType { get => movementType; }

    //Public Getters and Setters
    public ModularCurveContainer SetForceCurveContainer { set => forceCurveContainer = value; }
    public float ExtraForceMultiplier { get => extraForceMultiplier; set => extraForceMultiplier = value; }
    public float TimeOffset { get => timeOffset; set => timeOffset = value; }

    //Protected Getters
    protected Vector3 DirectionOscilating
    {
        get
        {
            if (directionXCurveContainer == null || directionYCurveContainer == null || directionZCurveContainer == null) return Vector3.zero;

            return new Vector3(
                directionXCurveContainer.Evaluate(Time.time + timeOffset),
                directionYCurveContainer.Evaluate(Time.time + timeOffset),
                directionZCurveContainer.Evaluate(Time.time + timeOffset)
            );
        }
    }

    protected virtual Vector3 LocalDirectionOscilating
    {
        get
        {
            return localDirection ? transform.TransformDirection(DirectionOscilating): DirectionOscilating;
        }
    }

    protected float ForceOscilating
    {
        get
        {
            if (forceCurveContainer == null) return 0;
            return forceCurveContainer.Evaluate(Time.time + timeOffset) * extraForceMultiplier;
        }
    }


    //Private
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();

        //rb.maxLinearVelocity = maxLinearVelocity;
        //rb.maxAngularVelocity = maxAngularVelocity;
    }
}
