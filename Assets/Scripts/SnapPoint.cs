using UnityEngine;
using System;

public class SnapPoint : MonoBehaviour
{
    private Collider coll;
    private ConfigurableJoint configurableJointReference;

    //PUBLIC
    public void SetConfigurableJointReference(ConfigurableJoint joint)
    {
        if (configurableJointReference != null) throw new Exception("configurableJointReference already set: " + configurableJointReference);
        configurableJointReference = joint;
    }

    public bool EnableCollider()
    {
        if (configurableJointReference != null) return false;
        coll.enabled = true;
        return true;
    }

    public void DisableCollider()
    {
        coll.enabled = false;
    }

    //PRIVATE
    private void Awake()
    {
        coll = GetComponent<Collider>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("SnapPoint"))
        {
            if (!coll.enabled) return;
            transform.parent.GetComponent<JointPointSnap>().Snap(other.gameObject, this);
        }
    }
}
