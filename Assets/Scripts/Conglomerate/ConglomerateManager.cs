using System.Collections.Generic;
using Unity.Loading;
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
    [SerializeField] private float liveTime = 60;
    [SerializeField] private float liveGainedFromSnap = 5;

    private float currentLiveTime = 0;

    private  List<Geobody> geobodies = new();

    //PUBLIC
    public void CreateConglomerate(Geobody geobody)
    {
        //if(geobodies.Count != 0) throw new System.Exception("Conglomerate already exists");
        geobodies.Clear();
        geobodies.Add(geobody);

        //Set currentLiveTIme
        currentLiveTime = liveTime;

        //Run once to set the first geobody to main head. //DONT RUN BECAUSE THE AMOUNT OF GEOBOIES IN THE LIST IS STILL FUCKED; CAN ONLY RUN AFTER THEOTHER GEOBODY IS ADDED
        //UpdateConglomerate();
    }

    public void CombineComglomerates(ConglomerateManager absorbedConglomerateManager)
    {
        //move all content from absorbed conglomerate manager to this conglomerate manager.
        geobodies.AddRange(absorbedConglomerateManager.geobodies);
        //absorbedConglomerateManager.geobodies.Clear(); idk remove bc i dont care anymore

        currentLiveTime += liveGainedFromSnap;

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

        currentLiveTime += liveGainedFromSnap;
        //Debug.Log("On joint snappped");
        UpdateConglomerate();
    }

    //PRIVATE
    private void UpdateConglomerate()
    {
        ConglomerateAnalyser.Instance.AnalyseConglomerate(geobodies, out conglomerateMovementData, out List<List<Geobody>> geobodyLayers);

        if(geobodies.Count == 0) throw new System.Exception("No geobodies in conglomerate");
        if(geobodies.Count == 1)
        {
            geobodies[0].SetToSeparate();
            return;
        }

        conglomerateMovementData.RunConglomerateAllotmentScript(this, geobodies, geobodyLayers);
    }

    private void Update()
    {
        if(geobodies.Count == 0) return;
        currentLiveTime -= Time.deltaTime;
        if (currentLiveTime > 0) return;
        List<Geobody> geobodiesCopy = new List<Geobody>(geobodies);
        geobodiesCopy.AddRange(geobodies);
        foreach (Geobody geobody in geobodiesCopy)
        {
            geobody.SetToSeparate();
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