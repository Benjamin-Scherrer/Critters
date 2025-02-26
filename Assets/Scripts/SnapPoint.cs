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

    private void OnTriggerEnter(Collider otherSnapPointCollider)
    {
        if (otherSnapPointCollider.CompareTag("SnapPoint"))
        {
            if (!coll.enabled) return;
            if(configurableJointReference != null) return;
            transform.parent.GetComponent<JointPointSnap>().Snap(otherSnapPointCollider.gameObject, this);
        }
    }


    //private void OnDestroy()
    //{
    //    throw new Exception("BRUH WTF.");
    //}
}
