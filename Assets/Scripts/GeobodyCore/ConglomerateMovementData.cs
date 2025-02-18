using UnityEngine;

public class ConglomerateMovementData : ScriptableObject
{
    [Header("Overarching")]
    [SerializeField] private AnimationCurve ForceAllocated = AnimationCurve.Linear(0, 0, 64, 64);
    [SerializeField] private AnimationCurve MainMovementFloatingMovementRatio = AnimationCurve.Constant(0, 64, 0.5f);

    [Header("Main Head Movement")]
    [SerializeField] private ModularCurveContainer mainHeadMainMovementContainer;
    [SerializeField] private AnimationCurve mainHeadMainMovementWeight = AnimationCurve.Constant(0, 64, 0.5f);
    [SerializeField] private ModularCurveContainer mainHeadFloatingMovementContainer;
    [SerializeField] private AnimationCurve mainHeadFloatingMovementWeight = AnimationCurve.Constant(0, 64, 0.5f);

    [Header("Side Head Movement")]
    [SerializeField] private ModularCurveContainer sideHeadMainMovementContainer;
    [SerializeField] private AnimationCurve sideHeadMainMovementOffset = AnimationCurve.Constant(0, 64, 0.5f);
    [SerializeField] private ModularCurveContainer sideHeadFloatingMovementContainer;
    [SerializeField] private AnimationCurve sideHeadFloatingMovementOffset = AnimationCurve.Constant(0, 64, 0.5f);
    [SerializeField] private int sideHeadInterval = 3;

    public ModularCurveContainer GetMainHeadMainMovementContainer { get => mainHeadMainMovementContainer; }
    public ModularCurveContainer GetMainHeadFloatingMovementContainer { get => mainHeadMainMovementContainer; }

    public ModularCurveContainer GetSideHeadMainMovementContainer { get => sideHeadMainMovementContainer; }
    public ModularCurveContainer GetSideHeadFloatingMovementContainer { get => sideHeadFloatingMovementContainer; }

    public int GetSideHeadInterval { get => sideHeadInterval; }

    public float EvaluateForceAllocated(int conglomerateSize)
    {
        return ForceAllocated.Evaluate(conglomerateSize);
    }

    public float EvaluateMainHeadMainMovementWeight(int conglomerateSize)
    {
        return mainHeadMainMovementWeight.Evaluate(conglomerateSize);
    }

    public float EvaluateMainHeadFloatingMovementWeight(int conglomerateSize)
    {
        return mainHeadFloatingMovementWeight.Evaluate(conglomerateSize);
    }

    public float EvaluateMainMovementFloatingMovementRatio(int conglomerateSize)
    {
        float mmfmr = MainMovementFloatingMovementRatio.Evaluate(conglomerateSize);
        if(mmfmr < 0 || mmfmr > 1) throw new System.Exception("Main Movement Floating Movement Ratio out of bounds");
        return mmfmr;
    }

    public float EvaluateSideHeadMovementOffset(int conglomerateSize)
    {
        return sideHeadMainMovementOffset.Evaluate(conglomerateSize);
    }

    public float EvaluateSideHeadFloatingMovementOffset(int conglomerateSize)
    {
        return sideHeadFloatingMovementOffset.Evaluate(conglomerateSize);
    }
}
