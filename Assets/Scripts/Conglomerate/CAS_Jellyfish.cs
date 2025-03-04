using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CAS_Jellyfish : CongomerateAllotmentScript
{
    public override void RunConglomerateAlotmentScript(ConglomerateManager conglomerateManager, ConglomerateMovementData conglomerateMovementData, List<Geobody> conglomerateGeobodies, List<List<Geobody>> geobodyLayers)
    {
        //code to find outermost geobodies
        List<Geobody> outermostGeobodies = FindOutermostGeobody(conglomerateGeobodies);

        //code to move find innermost geobodies by steping inside from the outermost geobodies 
        //assign inntermost geobody with multiple connections as main head

        Geobody mainHead = FindInnermostGeobody(outermostGeobodies, conglomerateGeobodies);

        ////TEST IF LAYERS ARE ALREADY FUCKED HERE
        //int conglomerateSize = conglomerateGeobodies.Count;
        ////DEBUG
        //int amountGeobodies = 0;
        //Debug.Log("Counting Geobodies: ");
        //for (int i = 0; i < geobodyLayers.Count; i++)
        //{
        //    List<Geobody> layer = geobodyLayers[i];
        //    if (layer == null) break; //is something with layers probably, incorrect cleanup????
        //    amountGeobodies += layer.Count;
        //    Debug.Log("Layer[" + i + "]: " + layer.Count);
        //}
        //Debug.Log("Amount of geobodies in layers: " + amountGeobodies);
        //Debug.Log("Amount of geobodies in conglomerate: " + conglomerateSize);
        //if (amountGeobodies != conglomerateSize) throw new System.Exception("Amount of geobodies in layers is not equal to the amount of geobodies in the conglomerate (HIGHLEVEL)");
        ////DEBUG END
        ////The jellyfish thinks its only one Gbody large... wtf

        //Set geobody layers from there on
        GoThroughConglomerateHierarchyStart(mainHead, geobodyLayers); //THIS HERE IS FUCKED


        ////TEST IF LAYERS ARE ALREADY FUCKED HERE
        ////DEBUG
        //amountGeobodies = 0;
        //Debug.Log("Counting Geobodies: ");
        //for (int i = 0; i < geobodyLayers.Count; i++)
        //{
        //    List<Geobody> layer = geobodyLayers[i];
        //    if (layer == null) break; //is something with layers probably, incorrect cleanup????
        //    amountGeobodies += layer.Count;
        //    Debug.Log("Layer[" + i + "]: " + layer.Count);
        //}
        //Debug.Log("Amount of geobodies in layers: " + amountGeobodies);
        //Debug.Log("Amount of geobodies in conglomerate: " + conglomerateSize);
        //if (amountGeobodies != conglomerateSize) throw new System.Exception("Amount of geobodies in layers is not equal to the amount of geobodies in the conglomerate (LOWLEVEL)");

        //COPY PASTED FROM CAS_Basic.cs
        AssignConglomerateRoles(conglomerateManager, conglomerateMovementData, conglomerateGeobodies.Count, geobodyLayers);
    }


    private List<Geobody> FindOutermostGeobody(List<Geobody> conglomerateGeobodies)
    {
        List<Geobody> outermostGeobodies = new List<Geobody>();
        foreach (Geobody geobody in conglomerateGeobodies)
        {
            if (geobody.GetSnappedGeobodies().Length == 1) outermostGeobodies.Add(geobody);
        }
        return outermostGeobodies;
    }

    private Geobody FindInnermostGeobody(List<Geobody> outermostGeobodies, List<Geobody> conglomerateGeobodies) 
    {
        List<List<Geobody>> geobodyLayers = new List<List<Geobody>>();
        
        geobodyLayers.Add(outermostGeobodies);
        for (int i = 0; i < 100; i++)
        {
            if(geobodyLayers.Count < i) break;
            List<Geobody> currentLayer = geobodyLayers[i];
            List<Geobody> nextLayer = new List<Geobody>();

            foreach(Geobody geobody in currentLayer)
            {
                Geobody[] snappedGeobodies = geobody.GetSnappedGeobodies();
                foreach (Geobody snappedGeobody in snappedGeobodies)
                {
                    if (CheckIfAlreadyBranchedTo(snappedGeobody, geobodyLayers)) continue;
                    nextLayer.Add(snappedGeobody);
                }
            }
            geobodyLayers.Add(nextLayer);
        }

        //magic vars but fuck me
        int innermostGeobodyIdealBranchCount = 4;
        int startIndex = geobodyLayers.Count - 1;

        Geobody innermostGeobody = null;
        int innermostGeobodyBranchCount = 0;

        for (int i = startIndex; i > 0; i--)
        {
            List<Geobody> currentLayer = geobodyLayers[i];
            foreach (Geobody geobody in currentLayer)
            {
                int geobodyGetSnappedGeobodiesLength = geobody.GetSnappedGeobodies().Length;
                if (geobodyGetSnappedGeobodiesLength < innermostGeobodyBranchCount) continue;
                innermostGeobody = geobody;
                innermostGeobodyBranchCount = geobodyGetSnappedGeobodiesLength;
                if (innermostGeobodyBranchCount >= innermostGeobodyIdealBranchCount) return innermostGeobody;
            }
        }
        return innermostGeobody;
    }

    private bool CheckIfAlreadyBranchedTo(Geobody gebody, List<List<Geobody>> geobodyLayers)
    {
        foreach (List<Geobody> geobodies in geobodyLayers)
        {
            if (geobodies == null) break;
            if (geobodies.IndexOf(gebody) != -1) return true;
        }
        return false;
    }

    //Second lap through conglomerate to set side heads and limbs

    private void GoThroughConglomerateHierarchyStart(Geobody mainHead, List<List<Geobody>> geobodyLayersTemp)
    {
        List<Geobody> geobodiesAlreadyReachedTemp = new();

        foreach (List<Geobody> geobodyLayerTemp in geobodyLayersTemp)
        {
            geobodyLayerTemp.Clear();
        }

        //geobodiesAlreadyReachedTemp.Add(mainHead); //NO NEED FOR THAT

        //if (geobodyLayersTemp.Count == 0) geobodyLayersTemp.Add(new List<Geobody>()); //FUCK THIS TOO
        //geobodyLayersTemp[0].Add(mainHead);

        GoThroughConglomerateHierarchy(0, mainHead, geobodiesAlreadyReachedTemp, geobodyLayersTemp);

        if (geobodyLayersTemp[0].Count != 1) throw new System.Exception("GeobodyLayers has weird amount of goebodies in main head layer:" + geobodyLayersTemp[0].Count);
    }

    private void GoThroughConglomerateHierarchy(int chainDepth, Geobody originGeobody, List<Geobody> geobodiesAlreadyReached, List<List<Geobody>> geobodyLayers)
    {
        if (chainDepth < 0) throw new System.Exception("Depth out of bounds");
        if (originGeobody == null) throw new System.Exception("Geobody is null");
        if (geobodiesAlreadyReached.IndexOf(originGeobody) != -1) return; //FUCK ME THAT WAS IT LMAO
        geobodiesAlreadyReached.Add(originGeobody);

        if (geobodyLayers.Count <= chainDepth) geobodyLayers.Add(new List<Geobody>());
        geobodyLayers[chainDepth].Add(originGeobody);

        Geobody[] tempGeobodies = originGeobody.GetSnappedGeobodies();

        //start branching
        chainDepth++;
        foreach (Geobody geobody in tempGeobodies)
        {
            if (geobody == null) throw new System.Exception("Geobody is null");
            if (geobody == originGeobody) throw new System.Exception("Geobody is itself");
            GoThroughConglomerateHierarchy(chainDepth, geobody, geobodiesAlreadyReached, geobodyLayers);
        }
    }


    //Geobody Setup
    //COPY PASTED FROM CAS_Basic.cs
    //Standart Conglomerate Setup
    private void AssignConglomerateRoles(ConglomerateManager conglomerateManager, ConglomerateMovementData conglomerateMovementData, int conglomerateGeobodiesCount, List<List<Geobody>> geobodyLayers)
    {
        if (geobodyLayers.Count == 0) throw new System.Exception("geobodyLayers not filled");

        Geobody mainHead = geobodyLayers[0][0];

        //Force variable setup
        float forceAllocated = conglomerateMovementData.EvaluateForceAllocated(conglomerateGeobodiesCount);

        int sideHeadCount = CountSideHeads(conglomerateMovementData, geobodyLayers);
        bool hasSideHeads = sideHeadCount > 0;

        MainHeadSetup(conglomerateManager, conglomerateMovementData, conglomerateGeobodiesCount, mainHead, forceAllocated, hasSideHeads, out float mainHeadMovementMult);


        SideHeadAndLimbSetup(conglomerateManager, conglomerateMovementData, geobodyLayers, sideHeadCount, forceAllocated, mainHeadMovementMult);
    }

    private int CountSideHeads(ConglomerateMovementData conglomerateMovementData, List<List<Geobody>> geobodyLayers)
    {
        //Count side heads
        int sideHeadInterval = conglomerateMovementData.GetSideHeadInterval;
        int sideHeadCount = 0;
        for (int i = 1; i < geobodyLayers.Count; i++)
        {
            if (geobodyLayers[i].Count == 0) break;
            if (i % sideHeadInterval != 0) continue;
            sideHeadCount += geobodyLayers[i].Count;
        }
        return sideHeadCount;
    }

    //Geobody Setups
    private void MainHeadSetup(ConglomerateManager conglomerateManager, ConglomerateMovementData conglomerateMovementData, int conglomerateGeobodiesCount, Geobody mainHead, float movementForceAllocated, bool hasSideHeads, out float mainHeadForceAllocated)
    {
        //Main head variables
        //Main head movement Containers
        List<IMovementModule> mainHeadMovementModules = conglomerateMovementData.GetMainHeadMovementModules;

        //Main head movement multipliers
        float mainHeadMainMovementWeight = hasSideHeads ? conglomerateMovementData.EvaluateMainHeadSideHeadForceRatio(conglomerateGeobodiesCount) : 1;
        mainHeadForceAllocated = movementForceAllocated * mainHeadMainMovementWeight;

        int mainHeadMaxSnap = conglomerateMovementData.GetMainHeadMaxSnaps;
        //Set main head. //Calculate variables depending on the existence of side heads.
        mainHead.SetToMainHead(conglomerateManager, mainHeadMovementModules, mainHeadForceAllocated, mainHeadMaxSnap);
    }

    private void SideHeadAndLimbSetup(ConglomerateManager conglomerateManager, ConglomerateMovementData conglomerateMovementData, List<List<Geobody>> geobodyLayers, int sideHeadCount, float movementForceAllocated, float mainHeadMovementMult)
    {
        int sideHeadInterval = conglomerateMovementData.GetSideHeadInterval;

        //needs to start at 1 so it doesnt overwrite the mainHead.
        for (int i = 1; i < geobodyLayers.Count; i++)
        {
            if (geobodyLayers[i].Count == 0) break;
            if (i % sideHeadInterval == 0) SideHeadSetup(conglomerateManager, conglomerateMovementData, geobodyLayers[i], sideHeadCount, i / sideHeadInterval, movementForceAllocated, mainHeadMovementMult);
            else LimbSetup(conglomerateManager, conglomerateMovementData, geobodyLayers[i]);
        }
    }

    private void SideHeadSetup(ConglomerateManager conglomerateManager, ConglomerateMovementData conglomerateMovementData, List<Geobody> sideHeads, int sideHeadCount, int sideHeadIndex, float movementForceAllocated, float mainHeadMovementMult)
    {
        //CONTINUE: Calculate side head variables
        //Side head movement containers
        List<IMovementModule> sideHeadMovementModules = conglomerateMovementData.GetSideHeadMovementModules;

        float sideHeadForceAllocated = movementForceAllocated - mainHeadMovementMult;

        //Side Head Movement Offset
        float sideTimeOffset = conglomerateMovementData.GetSideHeadTimeOffset * sideHeadIndex;

        //Set all selected side heads.
        sideHeadForceAllocated /= sideHeadCount;

        int sideHeadMaxSnap = conglomerateMovementData.GetSideHeadMaxSnaps;
        foreach (Geobody g in sideHeads)
        {
            g.SetToSideHead(conglomerateManager, sideHeadMovementModules, sideHeadForceAllocated, sideHeadMaxSnap, sideTimeOffset);
        }
    }

    private void LimbSetup(ConglomerateManager conglomerateManager, ConglomerateMovementData conglomerateMovementData, List<Geobody> limbs)
    {
        int limbMaxSnap = conglomerateMovementData.GetLimbMaxSnaps; 
        foreach (Geobody g in limbs)
        {
            g.SetToLimb(conglomerateManager, limbMaxSnap);
        }
    }
}
