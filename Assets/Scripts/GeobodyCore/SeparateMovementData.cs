using System.Collections.Generic;
using UnityEngine;

public class SeparateMovementData : ScriptableObject
{
    [Header("Separate Movement")]
    [SerializeField] private List<MovementModuleHolder> MovementModules;
    [SerializeField][Range(0f,1f)] private float ForceAllocated = 1;

    public List<IMovementModule> GetMovementModules
    {
        get
        {
            List<IMovementModule> modules = new List<IMovementModule>();
            foreach (MovementModuleHolder holder in MovementModules)
            {
                modules.Add(holder.GetMovementModule);
            }
            return modules;
        }
    }

    public float GetForceAllocated { get => ForceAllocated; }
}
