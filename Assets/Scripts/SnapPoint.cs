using UnityEngine;
using System;

public class SnapPoint : MonoBehaviour
{
    private Collider coll;
    private ConfigurableJoint configurableJointReference;
    [SerializeField] private GameObject connectedSnappointsDisplay;
    [SerializeField] private GameObject openSnappointsDisplay;

    public bool HasJoint { get { return configurableJointReference != null; } }
    //PUBLIC
    public bool JointIsConnectingTo(Geobody geobody)
    {
        if (configurableJointReference == null) throw new Exception("configurableJointReference is null.");
        Geobody originGeobody = configurableJointReference.GetComponent<Geobody>();
        Geobody connectedGeobody = configurableJointReference.connectedBody.GetComponent<Geobody>();

        return originGeobody == geobody || connectedGeobody == geobody;

    }

    public void SaveRemoveConnection()
    {
        if (configurableJointReference == null) throw new Exception("configurableJointReference is null.");

        //this one does make every affected geobody check their hierarchy...
        JointPointSnap originJointPointSnap = configurableJointReference.GetComponent<JointPointSnap>();
        JointPointSnap connectedJointPointSnap = configurableJointReference.connectedBody.GetComponent<JointPointSnap>();
        originJointPointSnap.SaveRemoveJointPointSnapFromList(connectedJointPointSnap);
        connectedJointPointSnap.SaveRemoveJointPointSnapFromList(originJointPointSnap);

        Destroy(configurableJointReference);
        configurableJointReference = null;
        connectedSnappointsDisplay.SetActive(false);
    }

    public void RiskyRemoveConnection()
    {
        if (configurableJointReference == null) throw new Exception("configurableJointReference is null.");

        //The difference is that this one does not make the geobodies check their hierarchy...
        JointPointSnap originJointPointSnap = configurableJointReference.GetComponent<JointPointSnap>();
        JointPointSnap connectedJointPointSnap = configurableJointReference.connectedBody.GetComponent<JointPointSnap>();
        originJointPointSnap.RiskyRemoveJointPointSnapFromList(connectedJointPointSnap);
        connectedJointPointSnap.RiskyRemoveJointPointSnapFromList(originJointPointSnap);

        Destroy(configurableJointReference);
        configurableJointReference = null;
        connectedSnappointsDisplay.SetActive(false);
    }

    public void SetConfigurableJointReference(ConfigurableJoint joint)
    {
        if (configurableJointReference != null) throw new Exception("configurableJointReference already set: " + configurableJointReference);
        configurableJointReference = joint;
        connectedSnappointsDisplay.SetActive(true);
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
            if (configurableJointReference != null) return;
            transform.parent.GetComponent<JointPointSnap>().Snap(otherSnapPointCollider.gameObject, this);
        }
    }

    private void Update()
    {
        if (openSnappointsDisplay != null && connectedSnappointsDisplay != null)
        {
            openSnappointsDisplay.SetActive(!connectedSnappointsDisplay.activeSelf && Grabber.Instance.isGrabbing);
        }
    }


    //private void OnDestroy()
    //{
    //    throw new Exception("BRUH WTF.");
    //}
}
