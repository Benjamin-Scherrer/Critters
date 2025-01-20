using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Movement_Abstract : MonoBehaviour
{
    [Header("Base Variables")]
    [SerializeField] protected float force = 5f;
    [SerializeField] protected Vector3 direction = Vector3.up;
    [SerializeField] protected float maxLinearVelocity = 1;
    [SerializeField] protected float maxAngularVelocity = 1;

    [Header("Advanced Variables")]
    [SerializeField] protected float forceOscilationTime = 1;
    [SerializeField] protected float forceOscilationAmplitude = 1;

    [SerializeField] protected Vector3 directionOscilationTime = Vector3.one;
    [SerializeField] protected Vector3 directionOscilationAmplitude = Vector3.one;

    [Header("Debug")]
    [SerializeField][Range(0, 10)] protected float GizmoSize = 1;

    //References
    protected Rigidbody rb;
    protected virtual Vector3 LocalDirection
    {
        get
        {
            return transform.TransformDirection(direction);
        }
    }

    protected Vector3 LocalDirectionOscilating
    {
        get
        {
            return LocalDirection + new Vector3(
                Mathf.Sin(Time.time / directionOscilationTime.x) * directionOscilationAmplitude.x,
                Mathf.Sin(Time.time / directionOscilationTime.y) * directionOscilationAmplitude.y,
                Mathf.Sin(Time.time / directionOscilationTime.z) * directionOscilationAmplitude.z
            );
        }
    }

    protected float ForceOscilating
    {
        get
        {
            return force + Mathf.Sin(Time.time / forceOscilationTime) * forceOscilationAmplitude;
        }
    }


    //Private
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();

        rb.maxLinearVelocity = maxLinearVelocity;
        rb.maxAngularVelocity = maxAngularVelocity;
    }
}
