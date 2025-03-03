using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForceField : MonoBehaviour
{
    [Tooltip("The magnitude of force applied to rigidbodies within the trigger")]
    public float force = 0;
    [Tooltip("The direction of the force applied to rigidbodies within the trigger")]
    public Vector3 direction = Vector3.forward;
    [Tooltip("Whether to use local space for the direction of the force")]
    public bool useLocalSpace = true;

    [Space]
    [Tooltip("The variance in the magnitude of the force")]
    public float forceVariance = 0;
    [Tooltip("The rate of change for the force variance")]
    public float forceRateOfChange = 1;

    [Space]
    [Tooltip("The variance in the direction of the force")]
    public float directionVariance = 0;
    [Tooltip("The rate of change for the direction variance")]
    public float directionRateOfChange = 1;


    float forceNoiseSample = 0;
    float noiseForce = 0;

    Quaternion directionNoise = Quaternion.identity;


    void Update()
    {
        if (forceVariance != 0)
        {
            forceNoiseSample = Mathf.PerlinNoise1D(Time.time * forceRateOfChange);
            forceNoiseSample = forceNoiseSample * 2 - 1; // Normalize to -1 to 1
            noiseForce = forceNoiseSample * forceVariance;
        }
        if (directionVariance != 0)
        {
            directionNoise = Quaternion.Euler(
                (Mathf.PerlinNoise(Time.time * directionRateOfChange, 0) * 2 - 1) * directionVariance,
                (Mathf.PerlinNoise(Time.time * directionRateOfChange, 1) * 2 - 1) * directionVariance,
                (Mathf.PerlinNoise(Time.time * directionRateOfChange, 2) * 2 - 1) * directionVariance
            );
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent<Geobody>(out Geobody geobody))
        {
            geobody.leftForceField = true;
        }
    }

    void OnTriggerStay(Collider other)
    {

        if (other.TryGetComponent<Rigidbody>(out Rigidbody rb) && other.TryGetComponent<Geobody>(out Geobody geobody))
        {
            Vector3 appliedForceDirection = direction.normalized;

            if (geobody.wasOnScreen && geobody.leftForceField) appliedForceDirection = -direction.normalized;

            // Account for local space
            if (useLocalSpace)
            {
                appliedForceDirection = transform.TransformDirection(appliedForceDirection);
            }

            rb.AddForce(directionNoise * (appliedForceDirection * (force + noiseForce)));
        }
    }

    // This is only for the gizmo visualization
    void OnDrawGizmos()
    {
        // Set up color and coordinates
        Gizmos.color = Color.white;
        Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, transform.localScale);

        // Draw Box
        Gizmos.DrawWireCube(Vector3.zero, Vector3.one);

        // Draw Arrow
        float len = 0.8f + forceNoiseSample;
        Vector3 dir = directionNoise * direction;
        Vector3 pos = -len * 0.5f * dir;
        Gizmos.DrawRay(pos, dir * len);

        Vector3 right = Quaternion.LookRotation(dir) * Quaternion.Euler(45, 0, 0) * Vector3.back * len * 0.5f;
        Vector3 left = Quaternion.LookRotation(dir) * Quaternion.Euler(-45, 0, 0) * Vector3.back * len * 0.5f;
        Gizmos.DrawRay(pos + dir * len, right);
        Gizmos.DrawRay(pos + dir * len, left);

        // Reset coordinates
        Gizmos.matrix = Matrix4x4.identity;
    }
}
