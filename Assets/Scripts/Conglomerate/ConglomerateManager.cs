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


    private  List<Geobody> Geobodies = new();

    //PUBLIC
    public void CreateConglomerate(Geobody geobody)
    {
        if(Geobodies.Count != 0) throw new System.Exception("Conglomerate already exists");
        Geobodies.Add(geobody);

        //Run once to set the first geobody to main head.
        UpdateConglomerate();
    }

    public void CombineComglomerates(ConglomerateManager absorbedConglomerateManager)
    {
        //move all content from absorbed conglomerate manager to this conglomerate manager.
        Geobodies.AddRange(absorbedConglomerateManager.Geobodies);
        absorbedConglomerateManager.Geobodies.Clear();

        UpdateConglomerate();
    }

    //public void DestroyConglemerate()
    //{
    //    //Doesn't work yet and not important rn.
    //    //foreach (Geobody geobody in Geobodies)
    //    //{
    //    //    if (geobody.GetConglomerateHead != this) continue;
    //    //    geobody.SetToSeparate();
    //    //}

    //    Geobodies.Clear();
    //}

    public void OnJointSnapped(Geobody geobody)
    {
        if(Geobodies.IndexOf(geobody) != -1) return;

        Geobodies.Add(geobody);
        //geobody.JointSnapped += OnJointSnapped;

        //geobody.SetToLimb(); not really needed as the thing will go through the hierarchy anyway.

        //Debug.Log("On joint snappped");
        UpdateConglomerate();
    }

    //PRIVATE
    private void UpdateConglomerate()
    {
        ConglomerateAnalyser.Instance.AnalyseConglomerate(Geobodies, out conglomerateMovementData, out List<List<Geobody>> geobodyLayers);

        conglomerateMovementData.RunConglomerateAllotmentScript(this, Geobodies, geobodyLayers);
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