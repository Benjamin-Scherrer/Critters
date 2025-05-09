using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class ConglomerateManager : MonoBehaviour
{
    //The main head is decided at random by the order the geobodies connect.
    //From there all connections are followed and every few limbs a side head is assigned,
    //creating a node tree with differnet movmeent types.

    //change of plans, calculate force needed to move whole body, then give head a percentage of the power and to the limbs
    [Header("Data")]
    [SerializeField] private ConglomerateMovementData conglomerateMovementData;
    [Header("Death Timer")]
    [SerializeField] private float minLifeTime = 30;
    [SerializeField] private float maxLifeTime = 90;
    [SerializeField] private float lifeGainedFromSnap = 5;
    [SerializeField] private float lifeBufferFromClick = 2;
    [SerializeField] private float outsideScreenDecayMultiplier = 4;
    [SerializeField] private float deathExplosionForce = 10;
    [Header("Variables")]
    [SerializeField] private float idleGeobodyCount = 10;
    [SerializeField] private float maxGeobodyCount = 18;

    [Header("Debug")]
    [SerializeField] private float geobodyCount;

    private float currentLiveTime;

    private float lifeBuffer = 0;

    private  List<Geobody> geobodies = new();

    private bool isGrabbed = false;
    public bool IsGrabbed
    {
        get => isGrabbed;
        set
        {
            isGrabbed = value;
            lifeBuffer = lifeBufferFromClick;
        }
    }

    //PUBLIC
    public void CreateConglomerate(Geobody geobody)
    {
        //if(geobodies.Count != 0) throw new System.Exception("Conglomerate already exists");
        geobodies.Clear();
        geobodies.Add(geobody);

        //Set currentLiveTIme
        currentLiveTime = UnityEngine.Random.Range(minLifeTime, maxLifeTime);

        //Run once to set the first geobody to main head. //DONT RUN BECAUSE THE AMOUNT OF GEOBOIES IN THE LIST IS STILL FUCKED; CAN ONLY RUN AFTER THEOTHER GEOBODY IS ADDED
        //UpdateConglomerate();
    }

    public void CombineComglomerates(ConglomerateManager absorbedConglomerateManager)
    {
        //move all content from absorbed conglomerate manager to this conglomerate manager.
        geobodies.AddRange(absorbedConglomerateManager.geobodies);
        //absorbedConglomerateManager.geobodies.Clear(); idk remove bc i dont care anymore

        currentLiveTime += lifeGainedFromSnap;

        UpdateConglomerate();
    }

    public void DeactivateConglomerateManager()
    {
        geobodies.Clear();
    }

    public void OnJointSplit(Geobody geobody)
    {
        //Redo the conglomerate calcualtion.
        //geobodies.Clear(); //jank workaround
        CreateConglomerate(geobody);
        UpdateConglomerate();
    }

    public void OnJointSnapped(Geobody geobody)
    {
        if(geobodies.IndexOf(geobody) != -1) return;
        geobodies.Add(geobody);

        currentLiveTime += lifeGainedFromSnap;
        //Debug.Log("On joint snappped");
        UpdateConglomerate();

        AudioManager.Instance.PlayRandomeeAwakeSound();
    }


    //GRABBING
    public void SetGrabbed()
    {
        isGrabbed = true;
        foreach (Geobody geobody in geobodies)
        {
            geobody.SetGrabbed();
        }
    }

    public void SetUngrabbed()
    {
        isGrabbed = false;
        foreach (Geobody geobody in geobodies)
        {
            geobody.SetUngrabbed();
        }
    }


    //PRIVATE
    private void UpdateConglomerate()
    {
        if (this == null) return; //this is a fix for a bug that i can't find the source of. (NullReferenceException: Object reference not set to an instance of an object)

        ConglomerateAnalyser.Instance.AnalyseConglomerate(geobodies, out conglomerateMovementData, out List<List<Geobody>> geobodyLayers);

        if(geobodies.Count == 0) throw new System.Exception("No geobodies in conglomerate");
        if(geobodies.Count == 1)
        {
            geobodies[0].SetToSeparate();
            return;
        }

        conglomerateMovementData.RunConglomerateAllotmentScript(this, geobodies, geobodyLayers);


        foreach(Geobody geobody in geobodies)
        {
            if(geobody.GetGeobodyType == GeobodyType.Separate) throw new System.Exception("Geobody is separate while part of conglomerate");
        }


        //Split if too big
        if (geobodies.Count < geobodyCount) return;

        //Just kill conglomerate instead.
        KillConglomerate();

        //make geobides jump apart
        //Vector3 explosionOrigin = CalculateConglomerateCenter(geobodies);
        //Explode(geobodies, explosionOrigin);


        //int randomInt1 = UnityEngine.Random.Range(0, geobodies.Count);
        //Geobody geobody1 = geobodies[randomInt1];
        //Geobody[] connectedGeobodies =  geobody1.GetSnappedGeobodies();
        //int randomInt2 = UnityEngine.Random.Range(0, connectedGeobodies.Length);
        //Geobody geobody2 = connectedGeobodies[randomInt2];
        //geobody1.SplitOffgeobody(geobody2);
    }

    private void Awake()
    {
        geobodyCount = idleGeobodyCount;
    }

    private void Update()
    {
        if (InputAdapter.Instance.idle)
        {
            if (geobodyCount == idleGeobodyCount) return;
            geobodyCount = idleGeobodyCount;
        }
        else
        {
            if (geobodyCount == maxGeobodyCount) return;
            geobodyCount = maxGeobodyCount;
        }

        if (geobodies.Count == 0) return;
        if(isGrabbed) return;

        if (lifeBuffer > 0)
        {
            lifeBuffer -= Time.deltaTime;
            return;
        }

        if (geobodies[0].isInKillZone) currentLiveTime -= outsideScreenDecayMultiplier * Time.deltaTime;
        else currentLiveTime -= Time.deltaTime;

        if (currentLiveTime > 0) return;
        //KIll CODE
        KillConglomerate();
    }

    private void KillConglomerate()
    {
        List<Geobody> geobodiesCopy = new(geobodies);
        foreach (Geobody geobody in geobodies)
        {
            if (geobody == null) continue;
            geobodiesCopy.Add(geobody);
        }

        Vector3 explosionOrigin = CalculateConglomerateCenter(geobodiesCopy);

        SetAlltoSeparate(geobodiesCopy);
        Explode(geobodiesCopy, explosionOrigin);

        //
        AudioManager.Instance.PlayDestructionSound();
    }

    private void SetAlltoSeparate(List<Geobody> geobodiesCopy)
    {
        foreach (Geobody geobody in geobodiesCopy)
        {
            geobody.SetToSeparate();
        }
    }

    private Vector3 CalculateConglomerateCenter(List<Geobody> geobodies)
    {
        Vector3 explosionOrigin = Vector3.zero;

        foreach (Geobody geobody in geobodies)
        {
            try
            {
                explosionOrigin += geobody.transform.position;
            }
            catch (System.Exception)
            {
                continue;
            }
        }
        explosionOrigin /= geobodies.Count;
        explosionOrigin.y = 0;
        return explosionOrigin;
    }

    private void Explode(List<Geobody> geobodies, Vector3 explosionOrigin)
    {
        foreach (Geobody geobody in geobodies)
        {
            geobody.Explode(explosionOrigin, deathExplosionForce);
        }
    }
}




//Not needed rn

//End error checks
//Commented for now: TODO: Uncomment when needed.
//if(geobodiesAlreadyReached.Count < Geobodies.Count)
//{
//    foreach (Geobody g in Geobodies)
//    {
//        if (geobodiesAlreadyReached.IndexOf(g) == -1) DebugUtilityBenjamin.Draw3DCross(g.transform.position, 0.1f, Color.magenta);
//        else 
//            DebugUtilityBenjamin.Draw3DCross(g.transform.position, 0.1f, Color.green);
//    }

//    throw new System.Exception("Not all geobodies reached. Amount of Geobodies reached: " + geobodiesAlreadyReached.Count 
//        + ", Geobodies needed:" + Geobodies.Count);

//    //This case can be reached by having a geobody that just connected to the conglomerate,
//    //but the conglomerate hasn't connected back yet,
//    //causing a confusion to happen, as this search does not find the new geobody.

//}

//TODO: Make snapping robust enough to make this possible.
////this one here is important though because that would be looping which is very bad. //not yet ~because the snapping isn't limited yet.
//if (geobodiesAlreadyReached.Count > Geobodies.Count)
//{
//    foreach (Geobody g in geobodiesAlreadyReached)
//    {
//        if (Geobodies.IndexOf(g) == -1) DebugUtilityBenjamin.Draw3DCross(g.transform.position, 0.1f, Color.magenta);
//        else
//            DebugUtilityBenjamin.Draw3DCross(g.transform.position, 0.1f, Color.green);
//    }

//    //Too many geobodies reached.
//    throw new System.Exception("Too many geobodies reached. Amount of Geobodies reached: " + geobodiesAlreadyReached.Count
//        + ", Geobodies needed:" + Geobodies.Count);
//}

//Debug.Log("End");