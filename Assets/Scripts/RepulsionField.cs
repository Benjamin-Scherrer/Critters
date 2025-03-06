using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RepulsionField : MonoBehaviour
{
    [Header("Settings")]    
    [SerializeField] private bool pushOnXAxis;
    [SerializeField] private bool pushOnYAxis;
    [SerializeField] private bool pushOnZAxis;

    [Header("References")]
    [SerializeField] private Transform forceOrigin;

    [Header("Variables")]
    [SerializeField] private float repulsionForce = 10f;

    private void OnTriggerStay(Collider other)
    {
        if (!other.TryGetComponent(out Geobody component)) return;
        Vector3 forceTargetPosition = component.GetForceTargetPosition;
        Vector3 vector3 = (forceTargetPosition - forceOrigin.position);
        if(!pushOnXAxis) vector3.x = 0;
        if(!pushOnYAxis) vector3.y = 0;
        if (!pushOnZAxis) vector3.z = 0;
        vector3.Normalize();
        vector3 *= repulsionForce * Time.deltaTime;
        other.attachedRigidbody.AddForceAtPosition(vector3, forceTargetPosition, ForceMode.Impulse);
    }
}
