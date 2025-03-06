using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(JointPointSnap))]
[RequireComponent(typeof(MovementModuleManager))]
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
    private MovementModuleManager movementModuleManager;
    private ConglomerateManager conglomerateManager;

    private ConglomerateManager conglomerateHead;

    //PROPERTIES
    public ConglomerateManager GetConglomerateHead { get { return conglomerateHead; } } 

    public static int colorCount = 0;

    public bool wasOnScreen = false;
    public bool isOnScreen = false;
    public bool isInKillZone = false;

    public bool leftForceField = false;

    public GeobodyType GetGeobodyType { get { return geobodyType; } }

    public Vector3 GetForceTargetPosition => movementModuleManager.GetForceTargetPosition;

    //PUBLIC
    //EXPLODE
    public void SetGrabbed() { movementModuleManager.IsGrabbed = true; }
    public void SetUngrabbed() { movementModuleManager.IsGrabbed = false; }

    public void Explode(Vector3 explosionOrigin, float explosionForce)
    {
        movementModuleManager.Exlpode(explosionOrigin, explosionForce);
    }

    public Geobody[] GetSnappedGeobodies()
    {
        return jointPointSnap.GetSnappedGeobodies();
    }

    public void OnJointSplit()
    {
        //important: This is called on its own conglomerate manager not caring whether it already has a head or not.
        conglomerateManager.OnJointSplit(this);
        conglomerateHead = conglomerateManager;
    }

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

    public void SplitOffgeobody(Geobody geobody)
    {
        jointPointSnap.SplitOffGeobody(geobody);
    }

    public void SetToSeparate() 
    {
        if(this == null) return; //this is a fix for a bug that i can't find the source of. (NullReferenceException: Object reference not set to an instance of an object)

        //consider some way of reseting the modular curve containers when separating,
        //rn its fine to keep it like that because it looks fun if the parts keep moving with their old movement sceme.
        //prepare force data
        List<IMovementModule> separateMovementModules = separateMovementData.GetMovementModules;
        float forceAllocated = separateMovementData.GetForceAllocated;


        geobodyType = GeobodyType.Separate;
        movementModuleManager.SetMovementModules(separateMovementModules, forceAllocated, 0);
        movementModuleManager.ResetStartupTime();

        if (separateMaterial != null && useDebugMaterials) myRenderer.material = separateMaterial;
        conglomerateHead = null;

        jointPointSnap.SplitOffAll();
        jointPointSnap.UpdateSnapPointStatus(true, false, 2);

        if (!GeobodyManager.Instance.looseGeobodies.Contains(this)) GeobodyManager.Instance.looseGeobodies.Add(this);
        if (GeobodyManager.Instance.snappedGeobodies.Contains(this)) GeobodyManager.Instance.snappedGeobodies.Remove(this);

        conglomerateManager.enabled = false;
        conglomerateManager.DeactivateConglomerateManager();
    }

    public void SetToMainHead(ConglomerateManager newConglomerateHead, List<IMovementModule> movementModules, float forceAllocated, int maxSnaps)
    {
        geobodyType = GeobodyType.MainHead;
        movementModuleManager.SetMovementModules(movementModules, forceAllocated, 0);
        movementModuleManager.ResetStartupTime();

        if (mainHeadMaterial != null && useDebugMaterials) myRenderer.material = mainHeadMaterial;
        conglomerateHead = newConglomerateHead;

        
        jointPointSnap.UpdateSnapPointStatus(true, true, maxSnaps);

        if (!GeobodyManager.Instance.snappedGeobodies.Contains(this)) GeobodyManager.Instance.snappedGeobodies.Add(this);
        if (GeobodyManager.Instance.looseGeobodies.Contains(this)) GeobodyManager.Instance.looseGeobodies.Remove(this);

        if (conglomerateManager == conglomerateHead)
        {
            conglomerateManager.enabled = true;
        }
    }

    public void SetToSideHead(ConglomerateManager newConglomerateHead, List<IMovementModule> movementModules, float forceAllocated, int maxSnaps, float timeOffset)
    {
        geobodyType = GeobodyType.SideHead;
        movementModuleManager.SetMovementModules(movementModules, forceAllocated, timeOffset);
        movementModuleManager.ResetStartupTime();

        if (sideHeadMaterial != null && useDebugMaterials) myRenderer.material = sideHeadMaterial;
        conglomerateHead = newConglomerateHead;

        jointPointSnap.UpdateSnapPointStatus(true, true, maxSnaps);

        if (!GeobodyManager.Instance.snappedGeobodies.Contains(this)) GeobodyManager.Instance.snappedGeobodies.Add(this);
        if (GeobodyManager.Instance.looseGeobodies.Contains(this)) GeobodyManager.Instance.looseGeobodies.Remove(this);

        if (conglomerateManager != conglomerateHead)
        {
            conglomerateManager.enabled = false;
            conglomerateManager.DeactivateConglomerateManager();
        }
    }

    public void SetToLimb(ConglomerateManager newConglomerateHead, int maxSnaps)
    {
        geobodyType = GeobodyType.Limb;
        movementModuleManager.SetMovementModules(null, 0, 0);
        movementModuleManager.ResetStartupTime();

        if (limbMaterial != null && useDebugMaterials) myRenderer.material = limbMaterial;
        conglomerateHead = newConglomerateHead;

        jointPointSnap.UpdateSnapPointStatus(true, true, maxSnaps);

        if (!GeobodyManager.Instance.snappedGeobodies.Contains(this)) GeobodyManager.Instance.snappedGeobodies.Add(this);
        if (GeobodyManager.Instance.looseGeobodies.Contains(this)) GeobodyManager.Instance.looseGeobodies.Remove(this);

        if (conglomerateManager != conglomerateHead)
        {
            conglomerateManager.enabled = false;
            conglomerateManager.DeactivateConglomerateManager();
        }
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
        movementModuleManager = GetComponent<MovementModuleManager>();
        if(movementModuleManager == null) throw new Exception("No movement module manager found");

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
        OnScreenCheck();
        OutsideDeletion();

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
                //Absorb other conglomerate
                CombineComglomerates(other.GetConglomerateHead);
                break;
            case GeobodyType.SideHead:
                //Absorb other conglomerate
                CombineComglomerates(other.GetConglomerateHead);
                break;
            case GeobodyType.Limb:
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
                //Absorb other conglomerate
                CombineComglomerates(other.GetConglomerateHead);
                break;
            case GeobodyType.SideHead:
                //Absorb other conglomerate
                CombineComglomerates(other.GetConglomerateHead);
                break;
            case GeobodyType.Limb:
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
                //Absorb other conglomerate
                CombineComglomerates(other.GetConglomerateHead);
                break;
            case GeobodyType.SideHead:
                //Absorb other conglomerate
                CombineComglomerates(other.GetConglomerateHead);
                break;
            case GeobodyType.Limb:
                //Absorb other conglomerate
                CombineComglomerates(other.GetConglomerateHead);
                break;
            default: throw new ArgumentOutOfRangeException();
        }
    }

    private void CreateConglomerate(Geobody otherGeobody)
    {
        //if(conglomerateManager.enabled) throw new Exception("ConglomerateManager already enabled");
        //conglomerateManager.enabled = true;

        //first set this geobody to head
        conglomerateManager.CreateConglomerate(this);
        conglomerateHead = conglomerateManager;
        //then add the other geobody to the conglomerate
        conglomerateHead.OnJointSnapped(otherGeobody);
    }

    private void CombineComglomerates(ConglomerateManager absorbedConglomerateManager)
    {
        conglomerateHead.CombineComglomerates(absorbedConglomerateManager);
    }

    private void AddToConglomerate(Geobody otherGeobody)
    {
        conglomerateHead.OnJointSnapped(otherGeobody);
    }

    private void OnScreenCheck()
    {
        float edgeOffset = 4f * Screen.height / (2 * Camera.main.orthographicSize);
        Vector3 screenPos = Camera.main.WorldToScreenPoint(transform.position);

        if (screenPos.x < Screen.width && screenPos.x > 0f && screenPos.y < Screen.height && screenPos.y > 0f)
        {
            wasOnScreen = true;
            isOnScreen = true;
        }
        else
        {
            isOnScreen = false;
        }

        if (screenPos.x > Screen.width + edgeOffset || screenPos.x < -edgeOffset || screenPos.y > Screen.height + edgeOffset || screenPos.y < -edgeOffset && wasOnScreen)
        {
            isInKillZone = true;
        }
        else
        {
            isInKillZone = false;
        }
    }

    private void OutsideDeletion()
    {
        if (!isInKillZone) return;

        if (GeobodyManager.Instance.looseGeobodies.Contains(this)) // && geobodyType == GeobodyType.Separate && conglomerateHead == null)
        {
            if (GetSnappedGeobodies().Length > 0)
            {
                //Debug.Log("Trying to delete a snapped geobody");
                //return;
                throw new Exception("Loose geobody has snapped geobodies");
            }
            GeobodyManager.Instance.looseGeobodies.Remove(this);
            Destroy(gameObject);
        }
    }
}
