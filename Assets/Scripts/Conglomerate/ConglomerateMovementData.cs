using UnityEngine;
using System.Collections.Generic;
using System;

public class ConglomerateMovementData : ScriptableObject
{
    [Header("Requirements")]
    [SerializeField] private int minConglomerateSize = 1;
    [SerializeField] private int maxConglomerateSize = 8;
    [SerializeField][Range(0f,1f)][Tooltip("0 = lots of branches, 1 = no branches.")] private float conglomerateBranchingCoef = 0.5f;

    //Replace with new MovementModule approach
    [Header("Overarching")]
    [SerializeField] private AnimationCurve ForceAllocated = AnimationCurve.Linear(0, 0, 64, 64);
    [SerializeField] private AnimationCurve MainHeadSideHeadForceRatio = AnimationCurve.Constant(0, 1, 0.5f);

    [Header("Main Head Movement")]
    [SerializeField] private List<MovementModuleHolder> MainHeadMovementModules;

    [Header("Side Head Movement")]
    [SerializeField] private List<MovementModuleHolder> SideHeadMovementModules;
    [Space]
    [SerializeField] private float sideHeadTimeOffset = 0.5f;
    [SerializeField] private int sideHeadInterval = 3;

    [Header("Special Conglomerate Alotment Script")]
    [SerializeReference] private CongomerateAllotmentScript congomerateAllotmentScript;

    public int GetMinConglomerateSize { get => minConglomerateSize; }
    public int GetMaxConglomerateSize { get => maxConglomerateSize; }
    public float GetConglomerateBranchingCoef { get => conglomerateBranchingCoef; }


    public List<IMovementModule> GetMainHeadMovementModules 
    { 
        get 
        { 
            List<IMovementModule> modules = new();
            foreach(MovementModuleHolder holder in MainHeadMovementModules)
            {
                modules.Add(holder.GetMovementModule);
            }
            return modules;
        }
    }
    public List<IMovementModule> GetSideHeadMovementModules
    { 
        get 
        {
            List<IMovementModule> modules = new();
            foreach (MovementModuleHolder holder in SideHeadMovementModules)
            {
                modules.Add(holder.GetMovementModule);
            }
            return modules;
        } 
    }

    public int GetSideHeadInterval { get => sideHeadInterval; }
    public float GetSideHeadTimeOffset { get => sideHeadTimeOffset; }

    public float EvaluateForceAllocated(int conglomerateSize)
    {
        return ForceAllocated.Evaluate(conglomerateSize);
    }
    public float EvaluateMainHeadSideHeadForceRatio(int conglomerateSize)
    {
        return MainHeadSideHeadForceRatio.Evaluate(conglomerateSize);
    }


    public void NormalizeMovementModuleMultipliers()
    {
        float mainHeadTotalWeight = 0;
        foreach (MovementModuleHolder holder in MainHeadMovementModules)
        {
            mainHeadTotalWeight += holder.GetWeight;
        }
        foreach (MovementModuleHolder holder in MainHeadMovementModules)
        {
            holder.SetForceMultiplier = holder.GetWeight / mainHeadTotalWeight;
        }

        float sideHeadTotalWeight = 0;
        foreach (MovementModuleHolder holder in SideHeadMovementModules)
        {
            sideHeadTotalWeight += holder.GetWeight;
        }
        foreach (MovementModuleHolder holder in SideHeadMovementModules)
        {
            holder.SetForceMultiplier = holder.GetWeight / sideHeadTotalWeight;
        }
    }

    public void RunConglomerateAllotmentScript(ConglomerateManager conglomerateMaanger, List<Geobody> conglomerateGeobodies, List<List<Geobody>> geobodyLayers)
    {
        if (congomerateAllotmentScript == null) throw new Exception("No Conglomerate Allotment Script assigned to Conglomerate Movement Data");
        congomerateAllotmentScript.RunConglomerateAlotmentScript(conglomerateMaanger, this, conglomerateGeobodies, geobodyLayers);
    }
}
