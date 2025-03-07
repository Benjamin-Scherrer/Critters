using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnapPull : MonoBehaviour
{
    [SerializeField] private float pullForce = 10;

    void OnTriggerStay(Collider otherMagnet)
    {
        if (otherMagnet.CompareTag("Magnet"))
        {

            Transform geobodyTransform = transform.parent.parent;
            Transform otherGeobodyTransform = otherMagnet.transform.parent.parent;
            Rigidbody otherRigidbody = otherGeobodyTransform.GetComponent<Rigidbody>();
            Geobody geobody = geobodyTransform.GetComponent<Geobody>();
            Geobody otherGeobody = otherGeobodyTransform.GetComponent<Geobody>();
            JointPointSnap jointPointSnap = geobodyTransform.GetComponent<JointPointSnap>();

            if (jointPointSnap.CheckIfSnappedToSameHirarchy(geobody, otherGeobody)) return;

            //from snappoint to snappoint
            otherRigidbody.AddForceAtPosition((transform.position - otherMagnet.transform.position).normalized * pullForce, otherMagnet.transform.position);
        }
    }
}
