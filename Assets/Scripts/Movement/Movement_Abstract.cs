using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public abstract class Movement_Abstract : MonoBehaviour
{
    [Header("General")]
    [SerializeField] protected MovementType movementType = MovementType.MainMovement;

    [Header("Force")]
    [SerializeField] protected ModularCurveContainer forceCurve;
    
    //[Header("Velocity")]
    //[SerializeField] protected float maxLinearVelocity = 10;
    //[SerializeField] protected float maxAngularVelocity = 10;

    [Header("Direction")]
    [SerializeField] protected bool localDirection = false;
    [SerializeField] protected ModularCurveContainer directionXCurve;
    [SerializeField] protected ModularCurveContainer directionYCurve;
    [SerializeField] protected ModularCurveContainer directionZCurve;

    [Header("Debug")]
    [SerializeField][Range(0, 10)] protected float GizmoSize = 1;

    //References
    protected Rigidbody rb;

    //Extra Variables
    private float timeOffset = 0;
    private float extraForceMultiplier = 1;
    public MovementType MovementType { get => movementType; }
    public float ExtraForceMultiplier { get => extraForceMultiplier; set => extraForceMultiplier = value; }
    public float TimeOffset { get => timeOffset; set => timeOffset = value; }

    protected Vector3 DirectionOscilating
    {
        get
        {
            if (directionXCurve == null || directionYCurve == null || directionZCurve == null) return Vector3.zero;

            return new Vector3(
                directionXCurve.Evaluate(Time.time + timeOffset),
                directionYCurve.Evaluate(Time.time + timeOffset),
                directionZCurve.Evaluate(Time.time + timeOffset)
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
            if (forceCurve == null) return 0;
            return forceCurve.Evaluate(Time.time + timeOffset) * extraForceMultiplier;
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
