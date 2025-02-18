using System.Collections.Generic;
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
        GoThroughConglomerateHierarchyStart();
    }

    public void CombineComglomerates(ConglomerateManager absorbedConglomerateManager)
    {
        //move all content from absorbed conglomerate manager to this conglomerate manager.
        Geobodies.AddRange(absorbedConglomerateManager.Geobodies);
        absorbedConglomerateManager.Geobodies.Clear();

        GoThroughConglomerateHierarchyStart();
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

        Debug.Log("On joint snappped");
        GoThroughConglomerateHierarchyStart();
    }

    //PRIVATE

    private void GoThroughConglomerateHierarchyStart()
    {
        if (Geobodies.Count == 0) throw new System.Exception("Conglomerate not created");

        //prepare variables
        List<Geobody> geobodiesAlreadyReached = new(Geobodies.Count);
        List<Geobody> sideHeads = new(Geobodies.Count);

        //Moevment Containers
        ModularCurveContainer mainHeadMainMovementContainer = conglomerateMovementData.GetMainHeadMainMovementContainer;
        ModularCurveContainer mainHeadFloatingMovementContainer = conglomerateMovementData.GetMainHeadFloatingMovementContainer;
        ModularCurveContainer sideHeadMainMovementContainer = conglomerateMovementData.GetSideHeadMainMovementContainer;
        ModularCurveContainer sideHeadFloatingMovementContainer = conglomerateMovementData.GetSideHeadFloatingMovementContainer;

        //Side Head Interval
        int sideHeadInterval = conglomerateMovementData.GetSideHeadInterval;

        //Force
        float forceAllocated = conglomerateMovementData.EvaluateForceAllocated(Geobodies.Count);

        //Calculate force mult for main head and side head, main movement and floating movement.
        float mainHeadMainMovementWeight = conglomerateMovementData.EvaluateMainHeadMainMovementWeight(Geobodies.Count);
        float mainHeadFloatingMovementWeight = conglomerateMovementData.EvaluateMainHeadFloatingMovementWeight(Geobodies.Count);

        float sideHeadMainMovementWeight = 1 - mainHeadMainMovementWeight;
        float sideHeadFloatingWeight = 1 - mainHeadFloatingMovementWeight;

        float mainMovementFloatingMovementRatio = conglomerateMovementData.EvaluateMainMovementFloatingMovementRatio(Geobodies.Count);

        float MainHeadMainMovementMult = forceAllocated * mainHeadMainMovementWeight * mainMovementFloatingMovementRatio;
        float MainHeadFloatingMult = forceAllocated * mainHeadFloatingMovementWeight * (1 - mainMovementFloatingMovementRatio);

        float SideHeadMainMovementMult = forceAllocated * sideHeadMainMovementWeight * mainMovementFloatingMovementRatio;
        float SideHeadFloatingMult = forceAllocated * sideHeadFloatingWeight * (1 - mainMovementFloatingMovementRatio);
        //need to watch out with the side heads. Need to divide the force allocated by the amount of side heads.

        //Side Head Movement Offset
        float sideHeadMainMovementOffset = conglomerateMovementData.EvaluateSideHeadMovementOffset(Geobodies.Count);
        float sideHeadFloatingMovementOffset = conglomerateMovementData.EvaluateSideHeadFloatingMovementOffset(Geobodies.Count);

        //**********Start**********

        //Start at main head. First segment of list.
        Geobody geobody = Geobodies[0];
        if(geobody == null) throw new System.Exception("Main head is null");
        geobody.SetToMainHead(this,
            mainHeadMainMovementContainer, mainHeadFloatingMovementContainer,
            MainHeadMainMovementMult, MainHeadFloatingMult);
        geobodiesAlreadyReached.Add(geobody);


        Geobody[] tempGeobodies = geobody.GetSnappedGeobodies();
        //Debug.Log("Starting to go through hierarchy. TempGeobodies.Lenght: " + tempGeobodies.Length);
        foreach (Geobody g in tempGeobodies)
        {
            //Debug.Log("before hierarchy step 1");
            if(g == null) throw new System.Exception("Geobody is null");
            //Debug.Log("before hierarchy step 2");
            if (g == geobody) 
            {
                Debug.Log("Geobody g is the same as the main head");
                continue;
            };   
            //Debug.Log("before hierarchy step 3");
            GoThroughConglomerateHierarchyStep(0, 0, 
                sideHeadInterval, sideHeads,
                g, geobodiesAlreadyReached,
                sideHeadMainMovementContainer, sideHeadFloatingMovementContainer);
        }


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

        Debug.Log("End");
    }

    private void GoThroughConglomerateHierarchyStep(int chainSegmentIndex, int chainDepth,
        int sideHeadInterval, List<Geobody> sideHeads,
        Geobody geobody, List<Geobody> geobodiesAlreadyReached,
        ModularCurveContainer sideHeadMainMovementContainer, ModularCurveContainer sideHeadFloatingMovementContainer)
    {
        //Debug.Log("Starting hierarchy step. chainSegmentIndex: " + chainSegmentIndex + ", chainDepth: " + chainDepth);
        if (chainSegmentIndex < 0 ) throw new System.Exception("Index out of bounds");
        if(chainSegmentIndex > sideHeadInterval) throw new System.Exception("Index out of bounds");
        if (chainDepth < 0) throw new System.Exception("Depth out of bounds");
        if(geobody == null) throw new System.Exception("Geobody is null");

        if (geobodiesAlreadyReached.IndexOf(geobody) != -1) //throw new System.Exception("Geobody already reached");
        {
            //Debug.LogFormat("Geobody already reached. chainSegmentIndex: {0}, chainDepth {1}.", chainSegmentIndex, chainDepth);
            //DebugUtilityBenjamin.Draw3DCross(geobody.transform.position + Vector3.one * 0.1f, 0.1f, Color.cyan);
            return;
        }
        geobodiesAlreadyReached.Add(geobody);

        chainSegmentIndex++;
        if (chainSegmentIndex < sideHeadInterval)
        {
            //Debug.Log("Set to limb");
            geobody.SetToLimb(this);
        }
        else
        {
            //Debug.Log("Set to Side Head");
            chainSegmentIndex = 0;
            chainDepth++;
            sideHeads.Add(geobody);
        }

        Geobody[] tempGeobodies = geobody.GetSnappedGeobodies();
        //Debug.Log("Going through hierarchy step. TempGeobodies.Lenght: " + tempGeobodies.Length);
        foreach (Geobody g in tempGeobodies)
        {
            if (g == null) throw new System.Exception("Geobody is null");
            if (g == geobody) continue;
            GoThroughConglomerateHierarchyStep(chainSegmentIndex, chainDepth,
                sideHeadInterval, sideHeads,
                g, geobodiesAlreadyReached,
                sideHeadMainMovementContainer, sideHeadFloatingMovementContainer);
        }
    }
}
