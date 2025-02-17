using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class JointPointSnap : MonoBehaviour
{
    //[SerializeField] private float snapDistanceMult = 1.5f;

    [Header("Joint Settings")]
    [Space]
    [SerializeField] private float posSpring = 40f;
    [SerializeField] private float posDamp = 10f;
    [SerializeField] private float rotSpring = 20f;
    [SerializeField] private float rotDamp = 5f;

    private Geobody geobody;
    //private Collider coll;
    private List<Collider> snappedColliders = new List<Collider>();
    //private List<Transform> unsnappedTransforms = new List<Transform>();
    //private List<Transform> snappedTransforms = new List<Transform>();


    //public event Geobody.GeobodyEventHandler JointSnapped;

    //PUBLIC
    public Geobody[] GetSnappedGeobodies()
    {
        foreach (var snappedCollider in snappedColliders)
        {
            if (snappedCollider == null)
            {
                snappedColliders.Remove(snappedCollider);
            }
        }
        Geobody[] geobodies = new Geobody[snappedColliders.Count];

        for (int i = 0; i < snappedColliders.Count; i++)
        {
            if (snappedColliders[i].TryGetComponent(out Geobody g))
            {
                geobodies[i] = g;
                continue;
            }
            Debug.Log("Geobody not found");
            //geobodies[i] = snappedColliders[i].GetComponentInParent<Geobody>();
        }
        return geobodies;
    }


    void Start()
    {
        //coll = GetComponent<Collider>();

        geobody = GetComponent<Geobody>();
    }

    public void Snap(Collider collider, SnapPoint snapPoint)
    {
        var parentCollider = collider.transform.parent.GetComponent<Collider>();
        if (parentCollider.attachedRigidbody == null) throw new Exception("Rigidbody not found");
        if (!parentCollider.TryGetComponent(out Geobody otherGeobody)) throw new Exception("Geobody not found");


        //if the hierachy check works this is not needed.
        if (!snappedColliders.Contains(parentCollider) && !CheckIfSnappedToSameHirarchy(geobody, otherGeobody))
        { 
            var joint = gameObject.AddComponent<ConfigurableJoint>();
            joint.autoConfigureConnectedAnchor = false;
            joint.connectedBody = parentCollider.attachedRigidbody;
            joint.anchor = Vector3.zero;
            joint.connectedAnchor = collider.transform.localPosition + (snapPoint.transform.localPosition.magnitude * collider.transform.localPosition.normalized);
            joint.rotationDriveMode = RotationDriveMode.Slerp;
            var posDrive = new JointDrive { positionSpring = posSpring, positionDamper = posDamp, maximumForce = Mathf.Infinity };
            var rotDrive = new JointDrive { positionSpring = rotSpring, positionDamper = rotDamp, maximumForce = Mathf.Infinity };
            joint.xDrive = posDrive;
            joint.yDrive = posDrive;
            joint.zDrive = posDrive;
            joint.slerpDrive = rotDrive;
            snappedColliders.Add(parentCollider);
            snapPoint.connected = true;
            snapPoint.coll.enabled = false;

            ForceInverseSnap(collider, snapPoint);
        }
        //Tell geobody that is has snapped to another geobody
        geobody.OnJointSnap(otherGeobody);
    }

    private bool CheckIfSnappedToSameHirarchy(Geobody geobody, Geobody otherGeobody)
    {
        ConglomerateManager cMOne = geobody.GetConglomerateHead;
        if (cMOne == null) return false;
        ConglomerateManager cMTwo = otherGeobody.GetConglomerateHead;
        if (cMTwo == null) return false;
        if (cMOne == cMTwo) return true;
        return false;
    }

    private void ForceInverseSnap(Collider collider, SnapPoint snapPoint)
    {
        //force inverse snap
        SnapPoint otherSnappoint = collider.GetComponent<SnapPoint>();
        otherSnappoint.transform.parent.GetComponent<JointPointSnap>().Snap(snapPoint.coll, otherSnappoint);
        //return;
        //dangerous loop, but should never happpen.
    }
}
