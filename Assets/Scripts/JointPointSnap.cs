using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class JointPointSnap : MonoBehaviour
{
    [Header("Joint Settings")]
    [SerializeField] private ConfigurableJoint jointPrefab;

    //[SerializeField] private float posSpring = 40f;
    //[SerializeField] private float posDamp = 10f;
    //[SerializeField] private float rotSpring = 20f;
    //[SerializeField] private float rotDamp = 5f;

    [Header("Joint Groups")]
    [SerializeField] private List<SnapPoint> mainSnapPoints = new();
    [SerializeField] private List<SnapPoint> secondarySnapPoints = new();

    //[Header("Snapping Limit")]
    private int maxSnaps = 4;

    private bool allowMainSnapPoints = true;
    private bool allowSecondarySnapPoints = true;

    //References
    private Geobody geobody;
    private List<Collider> snappedColliders = new();



    //FUCK ME JANK UTILITY
    public List<SnapPoint> GetMainSnapPoints {  get  { return mainSnapPoints; } }
    public List<SnapPoint> GetSondarySnapPoints { get { return secondarySnapPoints; } }

    public List<Collider> GetSnappedColliders { get => snappedColliders; }


    //timestamp float
    private float timeOfSplit = 0;
    [Header("Snapping Cooldown after Splitting")] 
    [SerializeField] private float timePastSplitNeeded = 4;

    public bool IsPastTImeOfSplit
    {
    get
        {
            return Time.time - timeOfSplit > timePastSplitNeeded;
        }
    }


    //PUBLIC
    public void UpdateSnapPointStatus(bool allowMainSnapPoints, bool allowSecondarySnapPoints, int newMaxSnaps)
    {
        this.allowMainSnapPoints = allowMainSnapPoints;
        this.allowSecondarySnapPoints = allowSecondarySnapPoints;
        maxSnaps = newMaxSnaps;
        UpdateSnapPointStatus();
    }

  
    //idk what this is for just coded it for good measure lmao
    public void SplitOffGeobody(Geobody splitOffGeobody)
    {
        //find joint and delete it
        foreach(SnapPoint snapPoint in secondarySnapPoints.Concat(mainSnapPoints))
        {
            if (!snapPoint.HasJoint) continue;
            if (! snapPoint.JointIsConnectingTo(splitOffGeobody)) continue;
            snapPoint.SaveRemoveConnection();
        }
    }

    public void SplitOffAll()
    {
        foreach (SnapPoint snapPoint in secondarySnapPoints.Concat(mainSnapPoints))
        {
            if (!snapPoint.HasJoint) continue;
            snapPoint.RiskyRemoveConnection();
        }
    }

    public void SaveRemoveJointPointSnapFromList(JointPointSnap jointPointSnap)
    {
        Collider collider = jointPointSnap.GetComponent<Collider>();
        snappedColliders.Remove(collider);
        timeOfSplit = Time.time;
        //update snap points
        UpdateSnapPointStatus();
        //Update Geobody. -> Allows safety as geobody hierarchy is checked. 
        geobody.OnJointSplit();
    }

    public void RiskyRemoveJointPointSnapFromList(JointPointSnap jointPointSnap)
    {
        Collider collider = jointPointSnap.GetComponent<Collider>();
        snappedColliders.Remove(collider);
        timeOfSplit = Time.time;
        //update snap points.
        UpdateSnapPointStatus();
        ////Update Geobody //no need for savety 
        //geobody.OnJointSplit();
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

    public void Snap(GameObject otherSnapPointCollider, SnapPoint snapPoint)
    {
        if(!otherSnapPointCollider.TryGetComponent(out SnapPoint otherSnapPoint)) throw new Exception("SnapPoint not found");
        if(!otherSnapPointCollider.transform.parent.TryGetComponent(out Collider otherParentCollider)) throw new Exception("Rigidbody not found");
        if(!otherSnapPointCollider.transform.parent.TryGetComponent(out Geobody otherParentGeobody)) throw new Exception("Geobody not found");
        if(!otherSnapPointCollider.transform.parent.TryGetComponent(out JointPointSnap otherParentJointPointSnap)) throw new Exception("JointPointSnap not found");

        //prevent snapping to the same geobody it just split off from.
        if (!IsPastTImeOfSplit || !otherParentJointPointSnap.IsPastTImeOfSplit) return;

        //General check if it is already snapped to the same thing, possible by weird edge cases, just abort.
        if (snappedColliders.Contains(otherParentCollider)) return; // throw new Exception("Collider already snapped");

        //Check if either of the geobodies have reached the max snaps
        if (otherParentJointPointSnap.snappedColliders.Count >= otherParentJointPointSnap.maxSnaps) return;
        if (this.snappedColliders.Count >= this.maxSnaps) return; // throw new Exception("Max snaps reached");

        //Hierarchy check - if of the same hierarchy, do not snap
        if (CheckIfSnappedToSameHirarchy(geobody, otherParentGeobody)) return;

        //create joint
        var joint = gameObject.AddComponent<ConfigurableJoint>();

        joint = SetJointPrefabSettings(joint);


        //first get direction of snapPoint
        Vector3 snapPointDirection = -snapPoint.transform.localPosition.normalized;

        // Choose an arbitrary vector that is not parallel to snapPointDirection
        Vector3 arbitraryVector = (Mathf.Abs(snapPointDirection.x) < 0.9f) ? Vector3.right : Vector3.up;

        // Calculate the first orthogonal vector
        joint.secondaryAxis = Vector3.Cross(snapPointDirection, arbitraryVector).normalized;

        // Calculate the second orthogonal vector
        joint.axis = Vector3.Cross(snapPointDirection, joint.secondaryAxis).normalized;
        //this is correct but for some reason the joint constraints wobble



        //sett other connected body
        joint.connectedBody = otherParentCollider.attachedRigidbody;

        //Set anchors
        joint.anchor = snapPoint.transform.localPosition;
        joint.connectedAnchor = otherSnapPointCollider.transform.localPosition;

        //joint.anchor = Vector3.zero;
        //joint.connectedAnchor = otherSnapPointCollider.transform.localPosition + (snapPoint.transform.localPosition.magnitude * otherSnapPointCollider.transform.localPosition.normalized);

        //joint.rotationDriveMode = RotationDriveMode.Slerp;


        //var posDrive = new JointDrive { positionSpring = posSpring, positionDamper = posDamp, maximumForce = Mathf.Infinity };
        //var rotDrive = new JointDrive { positionSpring = rotSpring, positionDamper = rotDamp, maximumForce = Mathf.Infinity };

        //joint.xDrive = posDrive;
        //joint.yDrive = posDrive;
        //joint.zDrive = posDrive;
        //joint.slerpDrive = rotDrive;

        if (snappedColliders.Count >= maxSnaps)
        {
            DisableMainSnapPoints();
            DisableSecondarySnapPointsColliders();
        }

        //Disable snapPoint once used. Set configurable joint reference to snapPoint,
        //so it can be used to check if the collider is disabled because of snapping
        //and which have it disabled because of other reasons
        snapPoint.SetConfigurableJointReference(joint);
        snapPoint.DisableCollider();
        snappedColliders.Add(otherParentCollider);

        SimulateInverseSnap(otherSnapPoint, otherParentJointPointSnap,  joint, snapPoint);
        //Tell geobody that is has snapped to another geobody
        geobody.OnJointSnap(otherParentGeobody);
    }


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

    private void SimulateInverseSnap(SnapPoint otherSnappoint, JointPointSnap otherParentJointPointSnap, ConfigurableJoint joint, SnapPoint snapPoint)
    {
        //Add joint reference to otherSnapPoint
        //disable otherSnapPoint collider
        otherSnappoint.SetConfigurableJointReference(joint);
        otherSnappoint.DisableCollider();

        //Add parent collider of snapPoint to snappedColliders of the parent of other SnapPoint
        otherParentJointPointSnap.snappedColliders.Add(snapPoint.transform.parent.GetComponent<Collider>());

        //No looping anymore!
        //Bad old code: otherSnappoint.transform.parent.GetComponent<JointPointSnap>().Snap(snapPoint.gameObject, otherSnappoint);
    }


    //SnapPoint Methods
    private void DisableMainSnapPoints()
    {
        foreach (SnapPoint snapPoint in mainSnapPoints)
        {
            snapPoint.DisableCollider();
        }
    }

    private void EnableMainSnapPoints()
    {
        foreach (SnapPoint snapPoint in mainSnapPoints)
        {
            snapPoint.EnableCollider();
        }
    }

    private void DisableSecondarySnapPointsColliders()
    {
        //Debug.Log("Disabling secondary snap points colliders");
        foreach (SnapPoint snapPoint in secondarySnapPoints)
        {
            //Debug.Log("//Disabled");
            snapPoint.DisableCollider();
        }
    }

    private void EnableSecondarySnapPointsColliders()
    {
        if (snappedColliders.Count >= maxSnaps)
        {
            //Debug.Log("Max snaps reached, cannot enable secondary snap points colliders");
            return;
        }
        foreach (SnapPoint snapPoint in secondarySnapPoints)
        {
            snapPoint.EnableCollider();
        }
    }

    //Original Damian Code

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


    private ConfigurableJoint SetJointPrefabSettings(ConfigurableJoint joint)
    {
        ////Useless assignement bc its overwriten later,
        //joint.connectedBody = jointPrefab.connectedBody;
        //joint.connectedArticulationBody = jointPrefab.connectedArticulationBody;

        ////Useless assignement bc its overwriten later,
        //joint.anchor = jointPrefab.anchor;

        //Special settings for that
        //joint.axis = jointPrefab.axis;

        //yes but needs to be always set to false, condiering to hardcode this.
        joint.autoConfigureConnectedAnchor = jointPrefab.autoConfigureConnectedAnchor;
        //joint.autoConfigureConnectedAnchor = false;

        ////Useless assignement bc its overwriten later,
        //joint.connectedAnchor = jointPrefab.connectedAnchor;

        //special settings for that
        //joint.secondaryAxis = jointPrefab.secondaryAxis;

        //probably set to limited
        joint.xMotion = jointPrefab.xMotion;
        joint.yMotion = jointPrefab.yMotion;
        joint.zMotion = jointPrefab.zMotion;

        //probably set to free
        joint.angularXMotion = jointPrefab.angularXMotion;
        joint.angularYMotion = jointPrefab.angularYMotion;
        joint.angularZMotion = jointPrefab.angularZMotion;

        //here go settings
        joint.linearLimitSpring = jointPrefab.linearLimitSpring;
        joint.linearLimit = jointPrefab.linearLimit;

        ////nothing bc of free //not free now
        joint.angularXLimitSpring = jointPrefab.angularXLimitSpring;
        joint.lowAngularXLimit = jointPrefab.lowAngularXLimit;
        joint.highAngularXLimit = jointPrefab.highAngularXLimit;
        joint.angularYZLimitSpring = jointPrefab.angularYZLimitSpring;
        joint.angularYLimit = jointPrefab.angularYLimit;
        joint.angularZLimit = jointPrefab.angularZLimit;

        ////will be vector3.zero to get to stable position -> So no assignement needed
        //joint.targetPosition = jointPrefab.targetPosition;
        //joint.targetVelocity = jointPrefab.targetVelocity;

        joint.xDrive = jointPrefab.xDrive;
        joint.yDrive = jointPrefab.yDrive;
        joint.zDrive = jointPrefab.zDrive;

        ////will be vector3.zero to get to stable position -> So no assignement needed
        //joint.targetRotation = jointPrefab.targetRotation;
        //joint.targetAngularVelocity = jointPrefab.targetAngularVelocity;

        //probably slerp drive
        joint.rotationDriveMode = jointPrefab.rotationDriveMode;

        ////nothing bc of slerp drive
        //joint.angularXDrive = jointPrefab.angularXDrive;
        //joint.angularYZDrive = jointPrefab.angularYZDrive;

        //filled with slerp drive data
        joint.slerpDrive = jointPrefab.slerpDrive;

        ////probably nothing bc of none
        //joint.projectionMode = jointPrefab.projectionMode;
        //joint.projectionDistance = jointPrefab.projectionDistance;
        //joint.projectionAngle = jointPrefab.projectionAngle;

        //probably none bc we need the anchors relative to the geobodies    
        joint.configuredInWorldSpace = jointPrefab.configuredInWorldSpace;

        //suss mogus maybe swap every update lololol
        joint.swapBodies = jointPrefab.swapBodies;

        //RN none but eventually maybe to make the geobodies break apart
        joint.breakForce = jointPrefab.breakForce;
        joint.breakTorque = jointPrefab.breakTorque;

        //probably yes
        joint.enableCollision = jointPrefab.enableCollision;
        joint.enablePreprocessing = jointPrefab.enablePreprocessing;

        //idk what this does.
        joint.massScale = jointPrefab.massScale;
        joint.connectedMassScale = jointPrefab.connectedMassScale;

        return joint;
    }

    //PRIVATE SNAP POINT CODE
    private void UpdateSnapPointStatus()
    {
        if (snappedColliders.Count >= maxSnaps)
        {
            DisableMainSnapPoints();
            DisableSecondarySnapPointsColliders();

            //toss away extra geobodies, starting with those on the secondary snap points.
            StartRemovingSnappedGeobodies();
        }

        if (allowMainSnapPoints) EnableMainSnapPoints();
        else DisableMainSnapPoints();

        if (allowSecondarySnapPoints) EnableSecondarySnapPointsColliders();
        else DisableSecondarySnapPointsColliders();
    }

    //Split Off Code
    private void StartRemovingSnappedGeobodies()
    {
        for (int i = 0; i < snappedColliders.Count; i++)
        {
            if (snappedColliders.Count <= maxSnaps) break;
            if (GoThroughSnapPointsAndRemoveConnectedGeobody(secondarySnapPoints)) continue;
            if (GoThroughSnapPointsAndRemoveConnectedGeobody(mainSnapPoints)) continue;
            break;
        }
        //No need to update snap point status, as the relevant geobodies will do that themselves.
    }

    private bool GoThroughSnapPointsAndRemoveConnectedGeobody(List<SnapPoint> snapPoints)
    {
        foreach(SnapPoint snapPoint in snapPoints)
        {
            if (!snapPoint.HasJoint) continue;
            snapPoint.SaveRemoveConnection();
            return true;
        }
        return false;
    }

}
