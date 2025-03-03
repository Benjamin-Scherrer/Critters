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

    private void OnTriggerStay(Collider other)
    {
        //other.attachedRigidbody.AddForce()
    }
}
