using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CAS_Snake : CongomerateAllotmentScript
{
    public override void RunConglomerateAlotmentScript(ConglomerateManager conglomerateManager, ConglomerateMovementData conglomerateMovementData, 
        List<Geobody> conglomerateGeobodies, List<List<Geobody>> geobodyLayers)
    {
        Geobody outermostGeobody = FindOutermostGeobody(geobodyLayers);

        List<Geobody> snakeChain = FindLongestBranch(outermostGeobody);

        SetSnakeChainOrder(geobodyLayers[0][0], snakeChain);

        List<Geobody> limbs = new();
        foreach(Geobody geobody in conglomerateGeobodies)
        {
            if (snakeChain.Contains(geobody)) continue;
            limbs.Add(geobody);
        }

        float forceAllocated = conglomerateMovementData.EvaluateForceAllocated(conglomerateGeobodies.Count);
        bool hasSideHeads = conglomerateMovementData.GetSideHeadInterval > snakeChain.Count;
        MainHeadSetup(conglomerateManager, conglomerateMovementData, conglomerateGeobodies, snakeChain, forceAllocated, hasSideHeads, out float mainHeadForceAllocated);

        SnakeSideHeadSetupAndLimbSearching(conglomerateManager, conglomerateMovementData, snakeChain, forceAllocated, mainHeadForceAllocated, limbs);

        LimbSetup(conglomerateManager, limbs);
    }


    private Geobody FindOutermostGeobody(List<List<Geobody>> geobodyLayers)
    {
        for (int i = geobodyLayers.Count - 1; i > 0; i--)
        {
            List<Geobody> geobodies = geobodyLayers[i];
            //Debug.Log("i: " + i + " gebodies.Count: " + geobodies.Count);
            if(geobodies.Count == 0) continue;
            return geobodies[0];
        }
        throw new Exception("No outermost geobody found");
    }

    //Find Snake
    private List<Geobody> FindLongestBranch(Geobody outermostGeobody)
    {
        List<Geobody> geobodiesAlreadyReached = new();

        List<Geobody> longestBranch = GoThroughConglomerateHierarchy(outermostGeobody, geobodiesAlreadyReached);

        if(longestBranch == null) throw new Exception("Longest branch is null");
        return longestBranch;
    }

    private List<Geobody> GoThroughConglomerateHierarchy(Geobody outermostGeobody, List<Geobody> geobodiesAlreadyReached)
    {
        if (outermostGeobody == null) throw new Exception("ÔutermostGeobody is null");
        if (geobodiesAlreadyReached.IndexOf(outermostGeobody) != -1) return null;
        geobodiesAlreadyReached.Add(outermostGeobody);

        Geobody[] tempGeobodies = outermostGeobody.GetSnappedGeobodies();

        List<Geobody> currentChain = null;

        foreach (Geobody geobody in tempGeobodies)
        {
            if (geobody == null) throw new Exception("Geobody is null");
            if (geobody == outermostGeobody) { Debug.Log("Geobody g is the same as the main head"); continue; }

            List<Geobody> tempChain = GoThroughConglomerateHierarchy(geobody, geobodiesAlreadyReached);

            if (tempChain == null) continue;

            if (currentChain != null && tempChain.Count < currentChain.Count) continue;
            currentChain = tempChain;
        }

        if (currentChain == null) currentChain = new List<Geobody>();
        currentChain.Add(outermostGeobody);;

        return currentChain;
    }

    private void SetSnakeChainOrder(Geobody mainHead, List<Geobody> snakeChain)
    {
        float distanceToStart = (mainHead.transform.position - snakeChain[0].transform.position).magnitude;
        float distanceToEnd = (mainHead.transform.position - snakeChain[snakeChain.Count - 1].transform.position).magnitude;

        if (distanceToStart > distanceToEnd) snakeChain.Reverse();
    }

    //Set Snake
    private void MainHeadSetup(ConglomerateManager conglomerateManager, ConglomerateMovementData conglomerateMovementData, List<Geobody> conglomerateGeobodies,
        List<Geobody> snakeChain, float forceAllocated, bool hasSideHeads, out float mainHeadForceAllocated)
    {
        //Main head variables
        List<IMovementModule> mainHeadMovementModules = conglomerateMovementData.GetMainHeadMovementModules;

        //Main head movement multipliers
        float mainHeadMainMovementWeight = hasSideHeads ? conglomerateMovementData.EvaluateMainHeadSideHeadForceRatio(conglomerateGeobodies.Count) : 1;
        mainHeadForceAllocated = forceAllocated * mainHeadMainMovementWeight;

        for (int i = 0; i < mainHeadMovementModules.Count; i++)
        {
            IMovementModule movementModule = mainHeadMovementModules[i];
            if(movementModule == null) throw new Exception("Movement module is null");
            if (movementModule.GetType() != typeof(MM_Snake)) continue;
            MM_Snake snakeMovementModule = (MM_Snake)movementModule;
            snakeMovementModule.SetSnakeMainHeadLocalDirection(snakeChain[1].transform);
            mainHeadMovementModules[i] = snakeMovementModule;
        }

        //Set main head. //Calculate variables depending on the existence of side heads.
        snakeChain[0].SetToMainHead(conglomerateManager, mainHeadMovementModules, mainHeadForceAllocated);
    }

    private void SnakeSideHeadSetupAndLimbSearching(ConglomerateManager conglomerateManager, ConglomerateMovementData conglomerateMovementData, List<Geobody> snakeChain, float forceAllocated, float mainHeadForceAllocated, List<Geobody> Limbs)
    {
        int sideHeadInterval = conglomerateMovementData.GetSideHeadInterval;
        int sideHeadCount = snakeChain.Count / sideHeadInterval;
        float sideHeadForceAllocated = (forceAllocated - mainHeadForceAllocated) / sideHeadCount;

        for (int i = 1; i < snakeChain.Count; i++)
        {
            Geobody geobody = snakeChain[i];
            if (i % sideHeadInterval == 0)
            {
                SideHeadSetup(conglomerateManager, conglomerateMovementData, geobody, snakeChain[i-1], i / sideHeadInterval, sideHeadCount, sideHeadForceAllocated);
                continue;
            }
            else
            {
                Limbs.Add(geobody);
            }
        }
    }

    private void SideHeadSetup(ConglomerateManager conglomerateManager, ConglomerateMovementData conglomerateMovementData, Geobody sideHead, Geobody previousGeobody, int sideHeadIndex, int sideHeadCount, float sideHeadForceAllocated)
    {
        //Side head movement containers
        List<IMovementModule> sideHeadMovementModules = conglomerateMovementData.GetSideHeadMovementModules;
        float sideTimeOffset = conglomerateMovementData.GetSideHeadTimeOffset * sideHeadIndex;

        for (int i = 0; i < sideHeadMovementModules.Count; i++)
        {
            IMovementModule movementModule = sideHeadMovementModules[i];
            if (movementModule.GetType() != typeof(MM_Snake)) continue;
            MM_Snake snakeMovementModule = (MM_Snake)movementModule;
            snakeMovementModule.SetSnakeSideHeadLocalDirection(previousGeobody.transform);
            sideHeadMovementModules[i] = snakeMovementModule;
        }

        sideHead.SetToSideHead(conglomerateManager, sideHeadMovementModules, sideHeadForceAllocated, sideTimeOffset);
    }

    private void LimbSetup(ConglomerateManager conglomerateManager, List<Geobody> limbs)
    {
        foreach (Geobody g in limbs)
        {
            g.SetToLimb(conglomerateManager);
        }
    }
}