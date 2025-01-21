using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Movement_Abstract : MonoBehaviour
{
    [Header("Force")]
    [SerializeField] protected AnimationCurve force = AnimationCurve.Constant(0, 1, 1);
    [SerializeField] protected float forceCycleDuration = 1;
    [SerializeField] protected float forceMultiplier = 1;

    [Header("Velocity")]
    [SerializeField] protected float maxLinearVelocity = 10;
    [SerializeField] protected float maxAngularVelocity = 10;


    [Header("Direction")]
    [SerializeField] protected bool localDirection = false;
    [SerializeField] protected AnimationCurve directionX = AnimationCurve.Constant(0, 1, 1);
    [SerializeField] protected AnimationCurve directionY = AnimationCurve.Constant(0, 1, 1);
    [SerializeField] protected AnimationCurve directionZ = AnimationCurve.Constant(0, 1, 1);
    [SerializeField] protected Vector3 directionCycleDuration = Vector3.one;
    [SerializeField] protected Vector3 directionCycleMultiplier = Vector3.one;

    //[Header("Velocity Limit")]
    //[SerializeField] protected float maxLinearVelocity = 10;
    //[SerializeField] protected float maxAngularVelocity = 10;

    [Header("Advanced Variables")]


    [Header("Debug")]
    [SerializeField][Range(0, 10)] protected float GizmoSize = 1;

    //References
    protected Rigidbody rb;
  
    protected Vector3 DirectionOscilating
    {
        get
        {
            float curveDurationX = directionX[directionX.length - 1].time;
            float timeMultiplierX = curveDurationX / directionCycleDuration.x;
            float timeX = (Time.time % directionCycleDuration.x) * timeMultiplierX;


            float curveDurationY = directionY[directionY.length - 1].time;
            float timeMultiplierY = curveDurationY / directionCycleDuration.y;
            float timeY = (Time.time % directionCycleDuration.y) * timeMultiplierY;
               
            float curveDurationZ = directionZ[directionZ.length - 1].time;
            float timeMultiplierZ = curveDurationZ / directionCycleDuration.z;
            float timeZ = (Time.time % directionCycleDuration.z) * timeMultiplierZ;

            return new Vector3(
                directionX.Evaluate(timeX),
                directionY.Evaluate(timeY),
                directionZ.Evaluate(timeZ)
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
            float curveDuration = force[force.length - 1].time;
            float timeMultiplier = curveDuration / forceCycleDuration;
            float time = (Time.time % forceCycleDuration) * timeMultiplier;
            float value = force.Evaluate(time);
            return value * forceMultiplier;
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
