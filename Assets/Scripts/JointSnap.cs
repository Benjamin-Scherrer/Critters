using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class JointSnap : MonoBehaviour
{
    [SerializeField] private float snapDistance = 2f;
    [SerializeField] private float posSpring = 40f;
    [SerializeField] private float posDamp = 10f;
    [SerializeField] private float rotSpring = 20f;
    [SerializeField] private float rotDamp = 5f;

    private Collider coll;
    private List<Collider> snappedColliders = new List<Collider>();

    void Start()
    {
        coll = GetComponent<Collider>();
    }

    void Update()
    {
        Collider[] collidersToSnap = Physics.OverlapSphere(transform.position, snapDistance);
        foreach (Collider collider in collidersToSnap)
        {
            if (!snappedColliders.Contains(collider) && collider.attachedRigidbody != null && collider.CompareTag("Geobody") && coll != collider)
            {
                var joint = gameObject.AddComponent<ConfigurableJoint>();
                joint.connectedBody = collider.attachedRigidbody;
                joint.anchor = Vector3.zero;
                joint.rotationDriveMode = RotationDriveMode.Slerp;
                var posDrive = new JointDrive { positionSpring = posSpring, positionDamper = posDamp, maximumForce = Mathf.Infinity };
                var rotDrive = new JointDrive { positionSpring = rotSpring, positionDamper = rotDamp, maximumForce = Mathf.Infinity };
                joint.xDrive = posDrive;
                joint.yDrive = posDrive;
                joint.zDrive = posDrive;
                joint.slerpDrive = rotDrive;
                snappedColliders.Add(collider);

            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, snapDistance);
    }
}
