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

    void Start()
    {
        coll = GetComponent<Collider>();
    }

    public void Snap(Collider collider, SnapPoint snapPoint)
    {
        if (collider.CompareTag("SnapPoint"))
        {
            var joint = gameObject.AddComponent<ConfigurableJoint>();
            joint.autoConfigureConnectedAnchor = false;
            joint.connectedBody = collider.transform.parent.GetComponent<Rigidbody>();
            joint.anchor = Vector3.zero;
            joint.connectedAnchor = collider.transform.localPosition + (snapPoint.transform.localPosition.magnitude * collider.transform.localPosition.normalized);
            joint.rotationDriveMode = RotationDriveMode.Slerp;
            var posDrive = new JointDrive { positionSpring = posSpring, positionDamper = posDamp, maximumForce = Mathf.Infinity };
            var rotDrive = new JointDrive { positionSpring = rotSpring, positionDamper = rotDamp, maximumForce = Mathf.Infinity };
            joint.xDrive = posDrive;
            joint.yDrive = posDrive;
            joint.zDrive = posDrive;
            joint.slerpDrive = rotDrive;
            snappedColliders.Add(collider);
            snapPoint.connected = true;
            snapPoint.coll.enabled = false;
        }
    }
}
