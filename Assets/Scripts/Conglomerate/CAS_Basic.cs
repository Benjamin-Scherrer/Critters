using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CAS_Basic : CongomerateAllotmentScript
{
    public override void RunConglomerateAlotmentScript(ConglomerateManager conglomerateManager, ConglomerateMovementData conglomerateMovementData, 
        List<Geobody> conglomerateGeobodies, List<List<Geobody>> geobodyLayers)
    {
        AssignConglomerateRoles(conglomerateManager, conglomerateMovementData, conglomerateGeobodies, geobodyLayers);
    }

    //Standart Conglomerate Setup
    private void AssignConglomerateRoles(ConglomerateManager conglomerateManager, ConglomerateMovementData conglomerateMovementData, List<Geobody> conglomerateGeobodies, List<List<Geobody>> geobodyLayers)
    {
        if (geobodyLayers.Count == 0) throw new System.Exception("geobodyLayers not filled");

        Geobody mainHead = geobodyLayers[0][0];

        //Force variable setup
        float forceAllocated = conglomerateMovementData.EvaluateForceAllocated(conglomerateGeobodies.Count);

        int sideHeadCount = CountSideHeads(conglomerateMovementData, geobodyLayers);
        bool hasSideHeads = sideHeadCount > 0;

        MainHeadSetup(conglomerateManager, conglomerateMovementData, conglomerateGeobodies, mainHead, forceAllocated, hasSideHeads, out float mainHeadMovementMult);

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
    private void MainHeadSetup(ConglomerateManager conglomerateManager, ConglomerateMovementData conglomerateMovementData, List<Geobody> conglomerateGeobodies, Geobody mainHead, float movementForceAllocated, bool hasSideHeads, out float mainHeadForceAllocated)
    {
        //Main head variables
        //Main head movement Containers
        List<IMovementModule> mainHeadMovementModules = conglomerateMovementData.GetMainHeadMovementModules;

        //Main head movement multipliers
        float mainHeadMainMovementWeight = hasSideHeads ? conglomerateMovementData.EvaluateMainHeadSideHeadForceRatio(conglomerateGeobodies.Count) : 1;
        mainHeadForceAllocated = movementForceAllocated * mainHeadMainMovementWeight;

        //Set main head. //Calculate variables depending on the existence of side heads.
        mainHead.SetToMainHead(conglomerateManager, mainHeadMovementModules, mainHeadForceAllocated);
    }

    private void SideHeadAndLimbSetup(ConglomerateManager conglomerateManager, ConglomerateMovementData conglomerateMovementData, List<List<Geobody>> geobodyLayers, int sideHeadCount, float movementForceAllocated, float mainHeadMovementMult)
    {
        int sideHeadInterval = conglomerateMovementData.GetSideHeadInterval;

        //needs to start at 1 so it doesnt overwrite the mainHead.
        for (int i = 1; i < geobodyLayers.Count; i++)
        {
            if (geobodyLayers[i].Count == 0) break;
            if (i % sideHeadInterval == 0) SideHeadSetup(conglomerateManager, conglomerateMovementData,  geobodyLayers[i], sideHeadCount, i / sideHeadInterval, movementForceAllocated, mainHeadMovementMult);
            else LimbSetup(conglomerateManager, geobodyLayers[i]);
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

        foreach (Geobody g in sideHeads)
        {
            g.SetToSideHead(conglomerateManager, sideHeadMovementModules, sideHeadForceAllocated, sideTimeOffset);
        }
    }

    private void LimbSetup(ConglomerateManager conglomerateManager, List<Geobody> limbs)
    {
        foreach (Geobody g in limbs)
        {
            g.SetToLimb(conglomerateManager);
        }
    }
}
