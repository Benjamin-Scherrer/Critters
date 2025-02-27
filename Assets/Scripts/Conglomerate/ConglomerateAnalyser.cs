using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConglomerateAnalyser : MonoBehaviour
{
    public static ConglomerateAnalyser Instance;


    private List<Geobody> geobodiesAlreadyReachedTemp;
    private List<List<Geobody>> geobodyLayersTemp;

    [Header("Conglomerate Movement Data")]
    [SerializeField] private List<ConglomerateMovementData> conglomerateMovementDatas;

    private void Awake()
    {
        Instance = this;

        geobodiesAlreadyReachedTemp = new List<Geobody>();
        geobodyLayersTemp = new List<List<Geobody>>();

        foreach(ConglomerateMovementData conglomerateMovementData in conglomerateMovementDatas)
        {
            conglomerateMovementData.NormalizeMovementModuleMultipliers();
        }
    }

    public void AnalyseConglomerate(List<Geobody> conglomerateGeobodies, out ConglomerateMovementData conglomerateMovementData, out List<List<Geobody>> geobodyLayers)
    {
        int conglomerateSize = conglomerateGeobodies.Count;

        GoThroughConglomerateHierarchyStart(conglomerateGeobodies, out geobodyLayers);

        int geobodyLayersCount = 0;
        foreach(List<Geobody> geobodyLayer in geobodyLayers)
        {
            if (geobodyLayer.Count == 0) break;
                geobodyLayersCount++;
        }
        conglomerateMovementData = FitToMovementData(conglomerateSize, geobodyLayersCount);

       
    }

    private void GoThroughConglomerateHierarchyStart(List<Geobody> conglomerateGeobodies, out List<List<Geobody>> geobodyLayers)
    { 
        geobodiesAlreadyReachedTemp.Clear();

        foreach(List<Geobody> geobodyLayerTemp in geobodyLayersTemp)
        {
            geobodyLayerTemp.Clear();
        }

        Geobody mainHead = conglomerateGeobodies[0];
        geobodiesAlreadyReachedTemp.Add(mainHead);

        if(geobodyLayersTemp.Count == 0) geobodyLayersTemp.Add(new List<Geobody>());
        geobodyLayersTemp[0].Add(mainHead);

        GoThroughConglomerateHierarchy(mainHead, geobodiesAlreadyReachedTemp, geobodyLayersTemp);
        geobodyLayers = geobodyLayersTemp;

        if (geobodyLayers[0].Count != 1) throw new System.Exception("GeobodyLayers has weird amount of goebodies in main head layer:" + geobodyLayers[0].Count);
    }

    private void GoThroughConglomerateHierarchy(Geobody mainHead, List<Geobody> geobodiesAlreadyReached, List<List<Geobody>> geobodyLayers)
    {
        //Go through hierarchy. Start at main head. First segment of list.
        if (mainHead == null) throw new System.Exception("Main head is null");

        Geobody[] tempGeobodies = mainHead.GetSnappedGeobodies();
        //Debug.Log("Starting to go through hierarchy. TempGeobodies.Lenght: " + tempGeobodies.Length);
        foreach (Geobody geobody in tempGeobodies)
        {
            //Debug.Log("before hierarchy step 1");
            if (geobody == null) throw new System.Exception("Geobody is null");
            //Debug.Log("before hierarchy step 2");
            if (geobody == mainHead)
            {
                Debug.Log("Geobody g is the same as the main head");
                continue;
            }
            //Debug.Log("before hierarchy step 3");
            GoThroughConglomerateHierarchyStep(0, geobody, geobodiesAlreadyReached, geobodyLayers);
        }
    }

    private void GoThroughConglomerateHierarchyStep(int chainDepth, Geobody originGeobody, List<Geobody> geobodiesAlreadyReached, List<List<Geobody>> sideHeadLayers)
    {
        if (chainDepth < 0) throw new System.Exception("Depth out of bounds");
        if (originGeobody == null) throw new System.Exception("Geobody is null");
        if (geobodiesAlreadyReached.IndexOf(originGeobody) != -1) return;
        geobodiesAlreadyReached.Add(originGeobody);

        chainDepth++;

        if (sideHeadLayers.Count <= chainDepth) sideHeadLayers.Add(new List<Geobody>());
        sideHeadLayers[chainDepth].Add(originGeobody);

        Geobody[] tempGeobodies = originGeobody.GetSnappedGeobodies();
        foreach (Geobody geobody in tempGeobodies)
        {
            if (geobody == null) throw new System.Exception("Geobody is null");
            if (geobody == originGeobody) throw new System.Exception("Geobody is itself");
            GoThroughConglomerateHierarchyStep(chainDepth, geobody, geobodiesAlreadyReached, sideHeadLayers);
        }
    }

    private ConglomerateMovementData FitToMovementData(int conglomerateSize, int geobodyLayersCount)
    {
        float conglomerateBranchingCoef = (float)geobodyLayersCount / (float)conglomerateSize; //the more layers the closer to 1, thus the less branches it has.

        float matchingCoef = -1;
        ConglomerateMovementData selectedConglomerateMovementData = null;
        foreach (ConglomerateMovementData conglomerateMovementData in conglomerateMovementDatas)
        {
             float tempMatchingCoef = MatchConglorateMovementDatarequirements(conglomerateMovementData, conglomerateSize, conglomerateBranchingCoef);
            if (tempMatchingCoef < matchingCoef) continue;
            matchingCoef = tempMatchingCoef;
            selectedConglomerateMovementData = conglomerateMovementData;
        }

        if(selectedConglomerateMovementData == null) throw new System.Exception("No movement data found");
        return selectedConglomerateMovementData;
    }

    private float MatchConglorateMovementDatarequirements(ConglomerateMovementData conglomerateMovementData, int conglomerateSize, float conglomerateBranchingCoef)
    {
        bool cmdBool = (conglomerateMovementData.GetMinConglomerateSize <= conglomerateSize && conglomerateMovementData.GetMaxConglomerateSize >= conglomerateSize);
        float conglomerateeSizeCoef = cmdBool ? 1 : 0;

        float conglomerateBranchingCoefDifference = Mathf.Abs(conglomerateBranchingCoef - conglomerateMovementData.GetConglomerateBranchingCoef);
        float conglomerateBranchingCoefAlignement = 1 - conglomerateBranchingCoefDifference;

        return (conglomerateBranchingCoefAlignement + conglomerateeSizeCoef) / 2f;
    }


    //TODO: Make it do certain special steps to selects better heads for specific conglomerateMovementDatas
    //TODO: Make it split off certain geobodies for specific conglomerateMovementDatas
    //TODO: Give the side heads more specific scripts / functions to work off so it is more controlled.
}
