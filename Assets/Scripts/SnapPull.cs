using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnapPull : MonoBehaviour
{
    [SerializeField] private float pullForce = 10;


    Transform geobodyTransform;
    Geobody geobody;
    JointPointSnap jointPointSnap;


    private void Start()
    {
        geobodyTransform = transform.parent.parent;
        geobody = geobodyTransform.GetComponent<Geobody>();
        jointPointSnap = geobodyTransform.GetComponent<JointPointSnap>();
    }

    void OnTriggerStay(Collider otherMagnet)
    {
        if (otherMagnet.CompareTag("Magnet"))
        {
            if (!jointPointSnap.IsPastTImeOfSplitOrSnap) return;

            Transform otherGeobodyTransform = otherMagnet.transform.parent.parent;
            Geobody otherGeobody = otherGeobodyTransform.GetComponent<Geobody>();

            if (jointPointSnap.CheckIfSnappedToSameHirarchy(geobody, otherGeobody)) return;

            Rigidbody otherRigidbody = otherGeobodyTransform.GetComponent<Rigidbody>();

            //from snappoint to snappoint
            otherRigidbody.AddForceAtPosition((transform.position - otherMagnet.transform.position).normalized * pullForce, otherMagnet.transform.position);
        }
    }
}
