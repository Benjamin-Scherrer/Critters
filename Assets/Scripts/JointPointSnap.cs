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

    private Collider coll;
    private List<Collider> snappedColliders = new List<Collider>();
    private List<Transform> unsnappedTransforms = new List<Transform>();
    private List<Transform> snappedTransforms = new List<Transform>();


    public event Geobody.GeobodyEventHandler JointSnapped;

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
            if (TryGetComponent(out Geobody g))
            {
                geobodies[i] = g;
                continue;
            }
            geobodies[i] = snappedColliders[i].GetComponentInParent<Geobody>();
        }
        return geobodies;
    }


    void Start()
    {
        coll = GetComponent<Collider>();
    }

    public void Snap(Collider collider, SnapPoint snapPoint)
    {
        var parentCollider = collider.transform.parent.GetComponent<Collider>();
        if (!snappedColliders.Contains(parentCollider))
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

            //Tell the geobody that another geobody has been snapped to it.
            //Somewhat bulky, but it works. Look if there's a saver way to do this.
            if (collider.TryGetComponent(out Geobody g))
            {
                JointSnapped?.Invoke(g);
                return;
            }
            g = collider.GetComponentInParent<Geobody>();
            JointSnapped?.Invoke(g);
        }
    }
}
