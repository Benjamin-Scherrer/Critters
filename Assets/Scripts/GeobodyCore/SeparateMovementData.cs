using UnityEngine;

public class SeparateMovementData : ScriptableObject
{
    [Header("Separate Movement")]
    [SerializeField] private ModularCurveContainer separateMainMovementContainer;
    [SerializeField] private ModularCurveContainer separateFloatingMovementContainer;
    [SerializeField] private float separateMainMovementFloatingMovementRatio = 1;
    [SerializeField][Range(0f,1f)] private float ForceAllocated = 1;

    public ModularCurveContainer GetSeparateMainMovementContainer { get => separateMainMovementContainer; }
    public ModularCurveContainer GetSeparateFloatingMovementContainer { get => separateFloatingMovementContainer; }
    public float GetSeparateMainMovementFloatingMovementRatio { get => separateMainMovementFloatingMovementRatio; }

    public float GetForceAllocated { get => ForceAllocated; }
}
