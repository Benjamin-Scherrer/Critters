using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AwayOrienter : MonoBehaviour
{
    private JointPointSnap jointPointSnap;

    private void Start()
    {
        jointPointSnap = GetComponentInParent<JointPointSnap>();
    }


    private void FixedUpdate()
    {
        Vector3 repulsionSum = Vector3.zero;

        foreach (Collider collider in jointPointSnap.GetSnappedColliders)
        {
            repulsionSum += (transform.position - collider.transform.position).normalized;
        }

        repulsionSum = repulsionSum.normalized;
        if (repulsionSum == Vector3.zero) return;
        transform.rotation = Quaternion.LookRotation(repulsionSum);
    }

}
