using System;
using UnityEngine;

[RequireComponent(typeof(JointPointSnap))]
[RequireComponent(typeof(Movement_PointForce))]
[RequireComponent(typeof(ConglomerateManager))]
public class Geobody : MonoBehaviour
{
    [Header("Debug Materials")]
    [SerializeField] private Material separateMaterial;
    [SerializeField] private Material mainHeadMaterial;
    [SerializeField] private Material sideHeadMaterial;
    [SerializeField] private Material limbMaterial;
    private Renderer myRenderer;


    //*******************************
    //VARIABLES
    private GeobodyType geobodyType = GeobodyType.Separate;

    //REFERENCES
    private JointPointSnap jointPointSnap;
    private Movement_Abstract[] movementScripts;
    private ConglomerateManager conglomerateManager;

    private ConglomerateManager conglomerateHead;

    public ConglomerateManager GetConglomerateHead { get { return conglomerateHead; } } 

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

    public void SetToSeparate()
    {
        geobodyType = GeobodyType.Separate;
        foreach (Movement_Abstract movementScript in movementScripts)
        {
            switch (movementScript.MovementType)
            {
                case MovementType.MainMovement:
                    movementScript.enabled = true;
                    movementScript.ExtraForceMultiplier = 1;
                    break;
                case MovementType.FloatingMovement:
                    movementScript.enabled = false;
                    //movementScript.ExtraForceMultiplier = 1;
                    break;
                default: throw new ArgumentOutOfRangeException();
            }
        }

        if(separateMaterial != null) myRenderer.material = separateMaterial;
        conglomerateHead = null;
    }

    public void SetToMainHead(ConglomerateManager newConglomerateHead, float mainMovementExtraForceMultiplier, float floatingExtraForceMultiplier)
    {
        geobodyType = GeobodyType.MainHead;
        foreach (Movement_Abstract movementScript in movementScripts)
        {
           switch(movementScript.MovementType)
            {
                case MovementType.MainMovement:
                    movementScript.enabled = true;
                    movementScript.ExtraForceMultiplier = mainMovementExtraForceMultiplier;
                    break;
                case MovementType.FloatingMovement:
                    movementScript.enabled = true;
                    movementScript.ExtraForceMultiplier = floatingExtraForceMultiplier;
                    break;
                default: throw new ArgumentOutOfRangeException();
            }
        }

        if(mainHeadMaterial != null) myRenderer.material = mainHeadMaterial;
        conglomerateHead = newConglomerateHead;
    }

    public void SetToSideHead(ConglomerateManager newConglomerateHead,  float mainMovementExtraForceMultiplier, float floatingExtraForceMultiplier)
    {
        geobodyType = GeobodyType.SideHead;
        foreach (Movement_Abstract movementScript in movementScripts)
        {
            switch (movementScript.MovementType)
            {
                case MovementType.MainMovement:
                    movementScript.enabled = true;
                    movementScript.ExtraForceMultiplier = mainMovementExtraForceMultiplier;
                    break;
                case MovementType.FloatingMovement:
                    movementScript.enabled = true;
                    movementScript.ExtraForceMultiplier = floatingExtraForceMultiplier;
                    break;
                default: throw new ArgumentOutOfRangeException();
            }
        }

        if(sideHeadMaterial != null) myRenderer.material = sideHeadMaterial;
        conglomerateHead = newConglomerateHead;
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
                    //movementScript.ExtraForceMultiplier = 1;
                    break;
                case MovementType.FloatingMovement:
                    movementScript.enabled = false;
                    //movementScript.ExtraForceMultiplier = 1;
                    break;
                default: throw new ArgumentOutOfRangeException();
            }
        }

        if(limbMaterial != null) myRenderer.material = limbMaterial;
        conglomerateHead = newConglomerateHead;
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


        //Set Geobody Type
        SetToSeparate();
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
                AddToConglomerate(other); //this will overwrite the other main head to limb.
                break;
            case GeobodyType.SideHead:
                //throw new Exception("MainHead can't snap to SideHead");
                AddToConglomerate(other); //this will overwrite the other main head to limb.
                break;
            case GeobodyType.Limb:
                //This case can happen because when two geobodies snap one of them will turn into a mainhead and turn the other into a limb. 
                //Thus triggering this case, in which nothing happens because the geobody was already snapped.

                ////Still there needs to be a save against two conglomerates snapping to each other.
                ////check if this conglomerate is the same as the other ones. Otherwise throw an error.
                //CheckIfIsAlreadyConnected(other); removed for noww, because now we're working with overwriting the other conglomerate.

                AddToConglomerate(other); //this will overwrite the other conglomerate to be subordinate to this conglomerate.
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
                AddToConglomerate(other); //this will overwrite the other side head to limb.
                break;
            case GeobodyType.SideHead:
                //throw new Exception("SideHead can't snap to SideHead");
                AddToConglomerate(other); //this will overwrite the other side head to limb.
                break;
            case GeobodyType.Limb:
                //This case can happen because when two geobodies snap, and one of them is a side head, it  will turn the other into a limb. 
                //Thus triggering this case, in which nothing happens because the geobody was already snapped.

                ////Still there needs to be a save against two conglomerates snapping to each other.
                ////check if this conglomerate is the same as the other ones. Otherwise throw an error.
                //CheckIfIsAlreadyConnected(other); removed for noww, because now we're working with overwriting the other conglomerate.

                AddToConglomerate(other); //this will overwrite the other conglomerate to be subordinate to this conglomerate.
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

                AddToConglomerate(other); //this will overwrite the other conglomerate to be subordinate to this conglomerate.
                break;
            case GeobodyType.SideHead:
                //This case can happen because when two geobodies snap, and one of them is a side head, it  will turn the other into a limb. 
                //Thus triggering this case, in which nothing happens because the geobody was already snapped.

                ////Still there needs to be a save against two conglomerates snapping to each other.
                ////check if this conglomerate is the same as the other ones. Otherwise throw an error.
                //CheckIfIsAlreadyConnected(other); removed for noww, because now we're working with overwriting the other conglomerate.

                AddToConglomerate(other); //this will overwrite the other conglomerate to be subordinate to this conglomerate.
                break;
            case GeobodyType.Limb:
                //This case can happen because when two geobodies snap, and one of them is a side head, it  will turn the other into a limb. 
                //Thus triggering this case, in which nothing happens because the geobody was already snapped.

                ////Still there needs to be a save against two conglomerates snapping to each other.
                ////check if this conglomerate is the same as the other ones. Otherwise throw an error.
                //CheckIfIsAlreadyConnected(other); removed for noww, because now we're working with overwriting the other conglomerate.

                AddToConglomerate(other); //this will overwrite the other conglomerate to be subordinate to this conglomerate.
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

    //the following two don't need any difference if i code it sensibly.
    private void AddToConglomerate(Geobody otherGeobody)
    {
        conglomerateHead.OnJointSnapped(otherGeobody);
    }


    //private void CheckIfIsAlreadyConnected(Geobody other)
    //{
    //    Geobody[] snappedGeobodies = GetSnappedGeobodies();

    //    if (Array.IndexOf(snappedGeobodies, other) == -1) throw new Exception("Not Already Connected. Foreign Geobody reached snapping stage without permission.");
    //}


    //private void NotifyMainHead(Geobody geobody)
    //{
    //    JointSnapped?.Invoke(geobody);
    //}
}
