using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(JointPointSnap))]
[RequireComponent(typeof(Movement_PointForce))]
[RequireComponent(typeof(ConglomerateManager))]
public class Geobody : MonoBehaviour
{
    [Header("Separate Movement Data")]
    [SerializeField] private SeparateMovementData separateMovementData;

    [Header("Debug Materials")]
    [SerializeField] private Material separateMaterial;
    [SerializeField] private Material mainHeadMaterial;
    [SerializeField] private Material sideHeadMaterial;
    [SerializeField] private Material limbMaterial;
    [SerializeField] private bool useDebugMaterials = false;
    private bool baseMaterialCheck = false;
    private Material baseMaterial;
    private Renderer myRenderer;

    //*******************************
    //VARIABLES
    private GeobodyType geobodyType = GeobodyType.Separate;

    //REFERENCES
    private JointPointSnap jointPointSnap;
    private Movement_Abstract[] movementScripts;
    private ConglomerateManager conglomerateManager;

    private ConglomerateManager conglomerateHead;

    //PROPERTIES
    public ConglomerateManager GetConglomerateHead { get { return conglomerateHead; } } 

    public static int colorCount = 0;

    //EVENTS
    //public delegate void GeobodyEventHandler(Geobody geobody);
    //public event GeobodyEventHandler JointSnapped;


    public Geobody[] GetSnappedGeobodies()
    {
        return jointPointSnap.GetSnappedGeobodies();
    }

    //PUBLIC
    public void OnJointSnap(Geobody other)
    {
        if (other.GetConglomerateHead != null && other.GetConglomerateHead == conglomerateHead) return; 
        // throw new Exception("WOW this is useless."); //performance improvement but rn unneeded and needs a check against null.

        switch (other.geobodyType)
        {
            case GeobodyType.Separate:
                OnOtherSeparate(other);
                break;
            case GeobodyType.MainHead:
                OnOtherMainHead(other);
                break;
            case GeobodyType.SideHead:
                OnOtherSideHead(other);
                break;
            case GeobodyType.Limb:
                OnOtherLimb(other);
                break;
            default: throw new Exception("Unknown geobody type");
        }
    }

    public void SetToSeparate() //consider some way of reseting the modular curve containers when separating, rn its fine to keep it like that because it looks fun if the parts keep moving with their old movement sceme.
    {
        //prepare force data

        float forceAllocated = separateMovementData.GetForceAllocated;
        float mainMovementForceMult = forceAllocated * separateMovementData.GetSeparateMainMovementFloatingMovementRatio;
        float floatingMovementForceMult = forceAllocated * (1 - separateMovementData.GetSeparateMainMovementFloatingMovementRatio);


        geobodyType = GeobodyType.Separate;
        foreach (Movement_Abstract movementScript in movementScripts)
        {
            switch (movementScript.MovementType)
            {
                case MovementType.MainMovement:
                    movementScript.enabled = true;
                    movementScript.SetForceCurveContainer = separateMovementData.GetSeparateMainMovementContainer;
                    movementScript.ExtraForceMultiplier = mainMovementForceMult;
                    //movementScript.TimeOffset = 0;
                    break;
                case MovementType.FloatingMovement:
                    movementScript.enabled = true;
                    movementScript.SetForceCurveContainer = separateMovementData.GetSeparateFloatingMovementContainer;
                    movementScript.ExtraForceMultiplier = floatingMovementForceMult;
                    //movementScript.TimeOffset = 0;
                    break;
                default: throw new ArgumentOutOfRangeException();
            }
        }

        if(separateMaterial != null && useDebugMaterials) myRenderer.material = separateMaterial;
        conglomerateHead = null;

        jointPointSnap.UpdateSnapPointStatus(true, false);

        if (!GeobodyManager.Instance.looseGeobodies.Contains(this)) GeobodyManager.Instance.looseGeobodies.Add(this);
        if (GeobodyManager.Instance.snappedGeobodies.Contains(this)) GeobodyManager.Instance.snappedGeobodies.Remove(this);
    }

    public void SetToMainHead(ConglomerateManager newConglomerateHead,
        ModularCurveContainer mainMovementContainer, ModularCurveContainer floatingMovementContainer,
        float mainMovementExtraForceMultiplier, float floatingExtraForceMultiplier)
    {
        geobodyType = GeobodyType.MainHead;
        foreach (Movement_Abstract movementScript in movementScripts)
        {
           switch(movementScript.MovementType)
            {
                case MovementType.MainMovement:
                    movementScript.enabled = true;
                    movementScript.SetForceCurveContainer = mainMovementContainer;
                    movementScript.ExtraForceMultiplier = mainMovementExtraForceMultiplier;
                    movementScript.TimeOffset = 0;
                    break;
                case MovementType.FloatingMovement:
                    movementScript.enabled = true;
                    movementScript.SetForceCurveContainer = floatingMovementContainer;
                    movementScript.ExtraForceMultiplier = floatingExtraForceMultiplier;
                    movementScript.TimeOffset = 0;
                    break;
                default: throw new ArgumentOutOfRangeException();
            }
        }

        if(mainHeadMaterial != null && useDebugMaterials) myRenderer.material = mainHeadMaterial;
        conglomerateHead = newConglomerateHead;

        
        jointPointSnap.UpdateSnapPointStatus(true, true);

        if (!GeobodyManager.Instance.snappedGeobodies.Contains(this)) GeobodyManager.Instance.snappedGeobodies.Add(this);
        if (GeobodyManager.Instance.looseGeobodies.Contains(this)) GeobodyManager.Instance.looseGeobodies.Remove(this);
    }

    public void SetToSideHead(ConglomerateManager newConglomerateHead,
        ModularCurveContainer mainMovementContainer, ModularCurveContainer floatingMovementContainer,
        float mainMovementExtraForceMultiplier, float floatingExtraForceMultiplier, 
        float mainMovementOffset, float floatingMovementOffset)
    {
        geobodyType = GeobodyType.SideHead;
        foreach (Movement_Abstract movementScript in movementScripts)
        {
            switch (movementScript.MovementType)
            {
                case MovementType.MainMovement:
                    movementScript.enabled = true;
                    movementScript.SetForceCurveContainer = mainMovementContainer;
                    movementScript.ExtraForceMultiplier = mainMovementExtraForceMultiplier;
                    movementScript.TimeOffset = mainMovementOffset;
                    break;
                case MovementType.FloatingMovement:
                    movementScript.enabled = true;
                    movementScript.SetForceCurveContainer = floatingMovementContainer;
                    movementScript.ExtraForceMultiplier = floatingExtraForceMultiplier;
                    movementScript.TimeOffset = floatingMovementOffset;
                    break;
                default: throw new ArgumentOutOfRangeException();
            }
        }

        if(sideHeadMaterial != null && useDebugMaterials) myRenderer.material = sideHeadMaterial;
        conglomerateHead = newConglomerateHead;

        jointPointSnap.UpdateSnapPointStatus(true, true);

        if (!GeobodyManager.Instance.snappedGeobodies.Contains(this)) GeobodyManager.Instance.snappedGeobodies.Add(this);
        if (GeobodyManager.Instance.looseGeobodies.Contains(this)) GeobodyManager.Instance.looseGeobodies.Remove(this);
    }

    public void SetToLimb(ConglomerateManager newConglomerateHead)
    {
        geobodyType = GeobodyType.Limb;
        foreach (Movement_Abstract movementScript in movementScripts)
        {
            switch (movementScript.MovementType)
            {
                case MovementType.MainMovement:
                    movementScript.enabled = false;
                    //movementScript.SetForceCurveContainer = mainMovementContainer;
                    //movementScript.ExtraForceMultiplier = 1;
                    //movementScript.TimeOffset = 0;
                    break;
                case MovementType.FloatingMovement:
                    movementScript.enabled = false;
                    //movementScript.SetForceCurveContainer = floatingMovementContainer;
                    //movementScript.ExtraForceMultiplier = 1;
                    //movementScript.TimeOffset = 0;
                    break;
                default: throw new ArgumentOutOfRangeException();
            }
        }

        if(limbMaterial != null && useDebugMaterials) myRenderer.material = limbMaterial;
        conglomerateHead = newConglomerateHead;

        jointPointSnap.UpdateSnapPointStatus(true, false);

        if (!GeobodyManager.Instance.snappedGeobodies.Contains(this)) GeobodyManager.Instance.snappedGeobodies.Add(this);
        if (GeobodyManager.Instance.looseGeobodies.Contains(this)) GeobodyManager.Instance.looseGeobodies.Remove(this);
    }


    //PRIVATE
    private void Awake()
    {
        //
        if (!TryGetComponent<JointPointSnap>(out jointPointSnap)) throw new Exception("No JointSnap component found.");

        //subscribe to joint snap event on startup.
        //jointPointSnap.JointSnapped += OnJointSnap;
        //removed because i'm referencing the geobody anyways.

        //
        movementScripts = GetComponentsInChildren<Movement_Abstract>();
        if(movementScripts.Length == 0) throw new Exception("No Movement_Abstract components found in children of Geobody");

        //
        if(!TryGetComponent<ConglomerateManager>(out conglomerateManager)) 
            throw new Exception("No ConglomerateManager component found in children of Geobody");

        conglomerateManager.enabled = false;

        //
        if(!TryGetComponent<Renderer>(out myRenderer))
            throw new Exception("No Renderer component found in children of Geobody");
    }

    private void Start()
    {
        //Set Geobody Type
        SetToSeparate();

        baseMaterial = GeobodyManager.Instance.PickMaterial();
        myRenderer.material = baseMaterial;
    }

    private void Update()
    {
        if (!useDebugMaterials && baseMaterialCheck)
        {
            myRenderer.material = baseMaterial;
        }
        baseMaterialCheck = useDebugMaterials;
    }

    //
    private void OnOtherSeparate(Geobody other)
    {
        switch (geobodyType)
        {
            case GeobodyType.Separate:
                CreateConglomerate(other);
                break;
            case GeobodyType.MainHead:
                AddToConglomerate(other);
                break;
            case GeobodyType.SideHead:
                AddToConglomerate(other);
                break;
            case GeobodyType.Limb:
                AddToConglomerate(other);
                break;
            default: throw new ArgumentOutOfRangeException();
        }
    }

    private void OnOtherMainHead(Geobody other) 
    {
        switch (geobodyType)
        {
            case GeobodyType.Separate:
                other.AddToConglomerate(this);
                break;
            case GeobodyType.MainHead:
                //throw new Exception("MainHead can't snap to MainHead");

                //Absorb other conglomerate
                CombineComglomerates(other.GetConglomerateHead);
                break;
            case GeobodyType.SideHead:
                //throw new Exception("MainHead can't snap to SideHead");

                //Absorb other conglomerate
                CombineComglomerates(other.GetConglomerateHead);
                break;
            case GeobodyType.Limb:
                //This case can happen because when two geobodies snap one of them will turn into a mainhead and turn the other into a limb. 
                //Thus triggering this case, in which nothing happens because the geobody was already snapped.

                ////Still there needs to be a save against two conglomerates snapping to each other.
                ////check if this conglomerate is the same as the other ones. Otherwise throw an error.
                //CheckIfIsAlreadyConnected(other); removed for noww, because now we're working with overwriting the other conglomerate.

                //Absorb other conglomerate
                CombineComglomerates(other.GetConglomerateHead);
                break;
            default: throw new ArgumentOutOfRangeException();
        }
    }

    private void OnOtherSideHead(Geobody other) 
    {
        switch (geobodyType)
        {
            case GeobodyType.Separate:
                other.AddToConglomerate(this);
                break;
            case GeobodyType.MainHead:
                //throw new Exception("SideHead can't snap to MainHead");

                //Absorb other conglomerate
                CombineComglomerates(other.GetConglomerateHead);
                break;
            case GeobodyType.SideHead:
                //throw new Exception("SideHead can't snap to SideHead");

                //Absorb other conglomerate
                CombineComglomerates(other.GetConglomerateHead);
                break;
            case GeobodyType.Limb:
                //This case can happen because when two geobodies snap, and one of them is a side head, it  will turn the other into a limb. 
                //Thus triggering this case, in which nothing happens because the geobody was already snapped.

                ////Still there needs to be a save against two conglomerates snapping to each other.
                ////check if this conglomerate is the same as the other ones. Otherwise throw an error.
                //CheckIfIsAlreadyConnected(other); removed for noww, because now we're working with overwriting the other conglomerate.

                //Absorb other conglomerate
                CombineComglomerates(other.GetConglomerateHead);
                break;
            default: throw new ArgumentOutOfRangeException();
        }
    }

    private void OnOtherLimb(Geobody other) 
    {
        switch (geobodyType)
        {
            case GeobodyType.Separate:
                other.AddToConglomerate(this);
                break;
            case GeobodyType.MainHead:
                //This case can happen because when two geobodies snap, and one of them is a side head, it  will turn the other into a limb. 
                //Thus triggering this case, in which nothing happens because the geobody was already snapped.

                ////Still there needs to be a save against two conglomerates snapping to each other.
                ////check if this conglomerate is the same as the other ones. Otherwise throw an error.
                //CheckIfIsAlreadyConnected(other); removed for noww, because now we're working with overwriting the other conglomerate.

                //Absorb other conglomerate
                CombineComglomerates(other.GetConglomerateHead);
                break;
            case GeobodyType.SideHead:
                //This case can happen because when two geobodies snap, and one of them is a side head, it  will turn the other into a limb. 
                //Thus triggering this case, in which nothing happens because the geobody was already snapped.

                ////Still there needs to be a save against two conglomerates snapping to each other.
                ////check if this conglomerate is the same as the other ones. Otherwise throw an error.
                //CheckIfIsAlreadyConnected(other); removed for noww, because now we're working with overwriting the other conglomerate.

                //Absorb other conglomerate
                CombineComglomerates(other.GetConglomerateHead);
                break;
            case GeobodyType.Limb:
                //This case can happen because when two geobodies snap, and one of them is a side head, it  will turn the other into a limb. 
                //Thus triggering this case, in which nothing happens because the geobody was already snapped.

                ////Still there needs to be a save against two conglomerates snapping to each other.
                ////check if this conglomerate is the same as the other ones. Otherwise throw an error.
                //CheckIfIsAlreadyConnected(other); removed for noww, because now we're working with overwriting the other conglomerate.

                //Absorb other conglomerate
                CombineComglomerates(other.GetConglomerateHead);
                break;
            default: throw new ArgumentOutOfRangeException();
        }
    }



    private void CreateConglomerate(Geobody otherGeobody)
    {
        if(conglomerateManager.enabled) throw new Exception("ConglomerateManager already enabled");
        conglomerateManager.enabled = true;

        //first set this geobody to head
        conglomerateManager.CreateConglomerate(this);
        //then add the other geobody to the conglomerate
        conglomerateHead.OnJointSnapped(otherGeobody);
    }

    private void CombineComglomerates(ConglomerateManager absorbedConglomerateManager)
    {
        conglomerateHead.CombineComglomerates(absorbedConglomerateManager);
    }

    //private void DestroyConglomerate()
    //{
    //    conglomerateManager.DestroyConglemerate();
    //}

    //the following two don't need any difference if i code it sensibly.
    private void AddToConglomerate(Geobody otherGeobody)
    {
        conglomerateHead.OnJointSnapped(otherGeobody);
    }
}
