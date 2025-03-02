using UnityEngine;
using System;

public class SnapPoint : MonoBehaviour
{
    private Collider coll;
    private ConfigurableJoint configurableJointReference;

    public bool HasJoint { get { return configurableJointReference != null; } }
    //PUBLIC
    public bool JointIsConnectingTo(Geobody geobody)
    {
        if (configurableJointReference == null) throw new Exception("configurableJointReference is null.");
        Geobody originGeobody = configurableJointReference.GetComponent<Geobody>();
        Geobody connectedGeobody = configurableJointReference.connectedBody.GetComponent<Geobody>();

        return originGeobody == geobody || connectedGeobody == geobody;
    }

    public void RemoveConnection()
    {
        if (configurableJointReference == null) throw new Exception("configurableJointReference is null.");

        Debug.Log("Split Off");
        //remove each other from list
        JointPointSnap originJointPointSnap = configurableJointReference.GetComponent<JointPointSnap>();
        JointPointSnap connectedJointPointSnap = configurableJointReference.connectedBody.GetComponent<JointPointSnap>();
        originJointPointSnap.RemoveJointPointSnapFromList(connectedJointPointSnap);
        connectedJointPointSnap.RemoveJointPointSnapFromList(originJointPointSnap);

        Destroy(configurableJointReference);
        configurableJointReference = null;
    }

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
