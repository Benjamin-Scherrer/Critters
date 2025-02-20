using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class JointPointSnap : MonoBehaviour
{
    [Header("Joint Settings")]
    [Space]
    [SerializeField] private float posSpring = 40f;
    [SerializeField] private float posDamp = 10f;
    [SerializeField] private float rotSpring = 20f;
    [SerializeField] private float rotDamp = 5f;

    [Header("Joint Groups")]
    [SerializeField] private List<SnapPoint> mainSnapPoints = new List<SnapPoint>();
    [SerializeField] private List<SnapPoint> secondarySnapPoints = new List<SnapPoint>();

    [Header("Snapping Limit")]
    [SerializeField] private int maxSnaps = 4;


    private Geobody geobody;
    private List<Collider> snappedColliders = new List<Collider>();

    //PUBLIC
    public void DisableSecondarySnapPointsColliders()
    {
        foreach (var snapPoint in secondarySnapPoints)
        {
            snapPoint.DisableCollider();
        }
    }

    public void EnableSecondarySnapPointsColliders()
    {
        foreach (var snapPoint in secondarySnapPoints)
        {
            snapPoint.EnableCollider();
        }
    }


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
        }
        return geobodies;
    }

    public void Snap(GameObject other, SnapPoint snapPoint)
    {
        var parentCollider = other.transform.parent.GetComponent<Collider>();
        if (parentCollider.attachedRigidbody == null) throw new Exception("Rigidbody not found");
        if (!parentCollider.TryGetComponent(out Geobody otherGeobody)) throw new Exception("Geobody not found");


        //if the hierachy check works this is not needed.
        if (!snappedColliders.Contains(parentCollider) && !CheckIfSnappedToSameHirarchy(geobody, otherGeobody))
        {
            var joint = gameObject.AddComponent<ConfigurableJoint>();
            joint.autoConfigureConnectedAnchor = false;
            joint.connectedBody = parentCollider.attachedRigidbody;
            joint.anchor = Vector3.zero;
            joint.connectedAnchor = other.transform.localPosition + (snapPoint.transform.localPosition.magnitude * other.transform.localPosition.normalized);
            joint.rotationDriveMode = RotationDriveMode.Slerp;
            var posDrive = new JointDrive { positionSpring = posSpring, positionDamper = posDamp, maximumForce = Mathf.Infinity };
            var rotDrive = new JointDrive { positionSpring = rotSpring, positionDamper = rotDamp, maximumForce = Mathf.Infinity };
            joint.xDrive = posDrive;
            joint.yDrive = posDrive;
            joint.zDrive = posDrive;
            joint.slerpDrive = rotDrive;
            snappedColliders.Add(parentCollider);
            if(snappedColliders.Count >= maxSnaps)
            {
                DisableSecondarySnapPointsColliders();
            }   

            //Disable snapPoint once used.... needs a better system for further development.
            //In the new version the side snap points should be disabled and only enabled when the geobody is turned into a main head or side head.
            //By setting a configurable joint reference I am able to see which snap points have their colliders disables because of snapping
            //and which have it disabled because of other reasons
            snapPoint.SetConfigurableJointReference(joint);
            snapPoint.DisableCollider();

            ForceInverseSnap(other, snapPoint);
            //Tell geobody that is has snapped to another geobody
            geobody.OnJointSnap(otherGeobody);
        }
    }

    //public void Snap(Collider otherCollider, SnapPoint snapPoint)
    //{
    //    var parentCollider = otherCollider.transform.parent.GetComponent<Collider>();
    //    if (parentCollider.attachedRigidbody == null) throw new Exception("Rigidbody not found");
    //    if (!parentCollider.TryGetComponent(out Geobody otherGeobody)) throw new Exception("Geobody not found");


    //    //if the hierachy check works this is not needed.
    //    if (!snappedColliders.Contains(parentCollider) && !CheckIfSnappedToSameHirarchy(geobody, otherGeobody))
    //    { 
    //        var joint = gameObject.AddComponent<ConfigurableJoint>();
    //        joint.autoConfigureConnectedAnchor = false;
    //        joint.connectedBody = parentCollider.attachedRigidbody;
    //        joint.anchor = Vector3.zero;
    //        joint.connectedAnchor = otherCollider.transform.localPosition + (snapPoint.transform.localPosition.magnitude * otherCollider.transform.localPosition.normalized);
    //        joint.rotationDriveMode = RotationDriveMode.Slerp;
    //        var posDrive = new JointDrive { positionSpring = posSpring, positionDamper = posDamp, maximumForce = Mathf.Infinity };
    //        var rotDrive = new JointDrive { positionSpring = rotSpring, positionDamper = rotDamp, maximumForce = Mathf.Infinity };
    //        joint.xDrive = posDrive;
    //        joint.yDrive = posDrive;
    //        joint.zDrive = posDrive;
    //        joint.slerpDrive = rotDrive;
    //        snappedColliders.Add(parentCollider);

    //        //Disable snapPoint once used.... needs a better system for further development.
    //        //In the new version the side snap points should be disabled and only enabled when the geobody is turned into a main head or side head.
    //        //By setting a configurable joint reference I am able to see which snap points have their colliders disables because of snapping
    //        //and which have it disabled because of other reasons
    //        snapPoint.SetConfigurableJointReference(joint);
    //        snapPoint.DisabbleCollider();

    //        ForceInverseSnap(otherCollider, snapPoint);
    //        //Tell geobody that is has snapped to another geobody
    //        geobody.OnJointSnap(otherGeobody);
    //    }
    //}

    //PRIVATE
    private void Awake()
    {
        if (mainSnapPoints.Count == 0) throw new Exception("No main snap points found");
        geobody = GetComponent<Geobody>();
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

    private void ForceInverseSnap(GameObject other, SnapPoint snapPoint)
    {
        //force inverse snap
        SnapPoint otherSnappoint = other.GetComponent<SnapPoint>();
        otherSnappoint.transform.parent.GetComponent<JointPointSnap>().Snap(snapPoint.gameObject, otherSnappoint);
        //return;
        //dangerous loop, but should never happpen.
    }

    //private void ForceInverseSnap(Collider collider, SnapPoint snapPoint)
    //{
    //    //force inverse snap
    //    SnapPoint otherSnappoint = collider.GetComponent<SnapPoint>();
    //    otherSnappoint.transform.parent.GetComponent<JointPointSnap>().Snap(snapPoint.coll, otherSnappoint);
    //    //return;
    //    //dangerous loop, but should never happpen.
    //}
}
