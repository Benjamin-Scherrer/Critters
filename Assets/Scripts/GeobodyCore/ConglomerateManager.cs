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

        AssignConglomerateRoles(geobodyLayers);
    }


    private void AssignConglomerateRoles(List<List<Geobody>> geobodyLayers)
    {
        if (geobodyLayers.Count == 0) throw new System.Exception("geobodyLayers not filled");

        Geobody mainHead = geobodyLayers[0][0];

        //Force variable setup
        float forceAllocated = conglomerateMovementData.EvaluateForceAllocated(Geobodies.Count);
        float mainMovementFloatingMovementRatio = conglomerateMovementData.EvaluateMainMovementFloatingMovementRatio(Geobodies.Count);
        float mainMovementForceAllocated = forceAllocated * mainMovementFloatingMovementRatio;
        float floatingMovementForceAllocated = forceAllocated * (1 - mainMovementFloatingMovementRatio);

        int sideHeadCount = CountSideHeads(geobodyLayers);
        bool hasSideHeads = sideHeadCount > 0;

        MainHeadSetup(mainHead, mainMovementForceAllocated, floatingMovementForceAllocated, hasSideHeads, out float mainHeadMainMovementMult, out float mainHeadFloatingMovementMult);

        SideHeadAndLimbSetup(geobodyLayers, sideHeadCount, mainMovementForceAllocated, floatingMovementForceAllocated, mainHeadMainMovementMult, mainHeadFloatingMovementMult);

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
    }

    private int CountSideHeads(List<List<Geobody>> geobodyLayers)
    {
        //Count side heads
        int sideHeadInterval = conglomerateMovementData.GetSideHeadInterval;
        int sideHeadCount = 0;
        for (int i = 1; i < geobodyLayers.Count; i++)
        {
            if(geobodyLayers[i].Count == 0) break;
            if (i % sideHeadInterval != 0) continue;
            sideHeadCount += geobodyLayers[i].Count;
        }
        return sideHeadCount;
    }

    private void MainHeadSetup(Geobody mainHead, float mainMovementForceAllocated, float floatingMovementForceAllocated, bool hasSideHeads,
        out float mainHeadMainMovementMult, out float mainHeadFloatingMovementMult)
    {
        //Main head variables
        //Main head movement Containers
        ModularCurveContainer mainHeadMainMovementContainer = conglomerateMovementData.GetMainHeadMainMovementContainer;
        ModularCurveContainer mainHeadFloatingMovementContainer = conglomerateMovementData.GetMainHeadFloatingMovementContainer;

        //Main head movement multipliers
        float mainHeadMainMovementWeight = hasSideHeads ? conglomerateMovementData.EvaluateMainHeadMainMovementWeight(Geobodies.Count) : 1;
        float mainHeadFloatingMovementWeight = hasSideHeads ? conglomerateMovementData.EvaluateMainHeadFloatingMovementWeight(Geobodies.Count) : 1;

        mainHeadMainMovementMult = mainMovementForceAllocated * mainHeadMainMovementWeight;
        mainHeadFloatingMovementMult = floatingMovementForceAllocated * mainHeadFloatingMovementWeight;

        //Set main head. //Calculate variables depending on the existence of side heads.
        mainHead.SetToMainHead(this,
            mainHeadMainMovementContainer, mainHeadFloatingMovementContainer,
            mainHeadMainMovementMult, mainHeadFloatingMovementMult);
    }

    private void SideHeadAndLimbSetup(List<List<Geobody>> geobodyLayers, int sideHeadCount, float mainMovementForceAllocated, float floatingMovementForceAllocated,
        float mainHeadMainMovementMult, float mainHeadFloatingMovementMult)
    {
        int sideHeadInterval = conglomerateMovementData.GetSideHeadInterval;

        //needs to start at 1 so it doesnt overwrite the mainHead.
        for (int i = 1; i < geobodyLayers.Count; i++)
        {
            if (geobodyLayers[i].Count == 0) break;
            if (i % sideHeadInterval == 0) SideHeadSetup(geobodyLayers[i], sideHeadCount, mainMovementForceAllocated, floatingMovementForceAllocated,
        mainHeadMainMovementMult, mainHeadFloatingMovementMult);
            else LimbSetup(geobodyLayers[i]);
        }
    }

    private void SideHeadSetup(List<Geobody> sideHeads, int sideHeadCount, float mainMovementForceAllocated, float floatingMovementForceAllocated, 
        float mainHeadMainMovementMult, float mainHeadFloatingMovementMult)
    {
        //CONTINUE: Calculate side head variables
        //Side head movement containers
        ModularCurveContainer sideHeadMainMovementContainer = conglomerateMovementData.GetSideHeadMainMovementContainer;
        ModularCurveContainer sideHeadFloatingMovementContainer = conglomerateMovementData.GetSideHeadFloatingMovementContainer;

        float SideHeadMainMovementMult = mainMovementForceAllocated - mainHeadMainMovementMult;
        float SideHeadFloatingMult = floatingMovementForceAllocated - mainHeadFloatingMovementMult;

        //Side Head Movement Offset
        float sideHeadMainMovementOffset = conglomerateMovementData.EvaluateSideHeadMovementOffset(Geobodies.Count);
        float sideHeadFloatingMovementOffset = conglomerateMovementData.EvaluateSideHeadFloatingMovementOffset(Geobodies.Count);

        //Set all selected side heads.
        SideHeadMainMovementMult /= sideHeads.Count;
        SideHeadFloatingMult /= sideHeads.Count;

        foreach (Geobody g in sideHeads)
        {
            g.SetToSideHead(this,
                sideHeadMainMovementContainer, sideHeadFloatingMovementContainer,
                SideHeadMainMovementMult, SideHeadFloatingMult,
                sideHeadMainMovementOffset, sideHeadFloatingMovementOffset);
        }
    }

    private void LimbSetup(List<Geobody> limbs)
    {
        foreach (Geobody g in limbs)
        {
            g.SetToLimb(this);
        }
    }

    //private void GoThroughConglomerateHierarchyStep(int chainSegmentIndex, int chainDepth,
    //    int sideHeadInterval, List<Geobody> sideHeads,
    //    Geobody geobody, List<Geobody> geobodiesAlreadyReached)
    //{
    //    //Debug.Log("Starting hierarchy step. chainSegmentIndex: " + chainSegmentIndex + ", chainDepth: " + chainDepth);
    //    if (chainSegmentIndex < 0 ) throw new System.Exception("Index out of bounds");
    //    if(chainSegmentIndex > sideHeadInterval) throw new System.Exception("Index out of bounds");
    //    if (chainDepth < 0) throw new System.Exception("Depth out of bounds");
    //    if(geobody == null) throw new System.Exception("Geobody is null");

    //    if (geobodiesAlreadyReached.IndexOf(geobody) != -1) //throw new System.Exception("Geobody already reached");
    //    {
    //        //Debug.LogFormat("Geobody already reached. chainSegmentIndex: {0}, chainDepth {1}.", chainSegmentIndex, chainDepth);
    //        //DebugUtilityBenjamin.Draw3DCross(geobody.transform.position + Vector3.one * 0.1f, 0.1f, Color.cyan);
    //        return;
    //    }
    //    geobodiesAlreadyReached.Add(geobody);

    //    chainSegmentIndex++;
    //    if (chainSegmentIndex < sideHeadInterval)
    //    {
    //        //Debug.Log("Set to limb");
    //        geobody.SetToLimb(this);
    //    }
    //    else
    //    {
    //        //Debug.Log("Set to Side Head");
    //        chainSegmentIndex = 0;
    //        chainDepth++;
    //        sideHeads.Add(geobody);
    //    }

    //    Geobody[] tempGeobodies = geobody.GetSnappedGeobodies();
    //    //Debug.Log("Going through hierarchy step. TempGeobodies.Lenght: " + tempGeobodies.Length);
    //    foreach (Geobody g in tempGeobodies)
    //    {
    //        if (g == null) throw new System.Exception("Geobody is null");
    //        if (g == geobody) continue;
    //        GoThroughConglomerateHierarchyStep(chainSegmentIndex, chainDepth,
    //            sideHeadInterval, sideHeads,
    //            g, geobodiesAlreadyReached);
    //    }
    //}



    //private void AssignConglomerateRolesStep(Geobody mainHead, List<Geobody> geobodiesAlreadyReached, List<Geobody> sideHeads)
    //{
    //    //Side Head Interval
    //    int sideHeadInterval = conglomerateMovementData.GetSideHeadInterval;

    //    //Go through hierarchy. Start at main head. First segment of list.
    //    if (mainHead == null) throw new System.Exception("Main head is null");

    //    Geobody[] tempGeobodies = mainHead.GetSnappedGeobodies();
    //    //Debug.Log("Starting to go through hierarchy. TempGeobodies.Lenght: " + tempGeobodies.Length);
    //    foreach (Geobody geobody in tempGeobodies)
    //    {
    //        //Debug.Log("before hierarchy step 1");
    //        if (geobody == null) throw new System.Exception("Geobody is null");
    //        //Debug.Log("before hierarchy step 2");
    //        if (geobody == mainHead)
    //        {
    //            Debug.Log("Geobody g is the same as the main head");
    //            continue;
    //        }
    //        //Debug.Log("before hierarchy step 3");
    //        GoThroughConglomerateHierarchyStep(0, 0,
    //            sideHeadInterval, sideHeads,
    //            geobody, geobodiesAlreadyReached);
    //    }
    //}
}
