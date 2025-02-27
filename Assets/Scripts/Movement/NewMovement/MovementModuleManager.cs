using System;
using System.Collections.Generic;
using UnityEngine;

public class MovementModuleManager : MonoBehaviour
{
    [SerializeField] private Transform forceTarget;
    private List<IMovementModule> movementModules;
    private Rigidbody rb;
    private float forceAllocated;
    private float timeOffset;

    //PUBLIC
    public void SetMovementModules(List<IMovementModule> movementModules, float forceAllocated, float timeOffset)
    {
        this.movementModules.Clear();
        if(movementModules != null) this.movementModules.AddRange(movementModules);
        this.forceAllocated = forceAllocated;
        this.timeOffset = timeOffset;
    }


    //PRIVATE
    private void Awake()
    {
        if (!TryGetComponent(out rb)) throw new Exception("Rigidbody not found");

        movementModules = new List<IMovementModule>(4);
    }


    // Update is called once per frame
    private void FixedUpdate()
    {
        if(movementModules.Count == 0) return;
        Debug.Log("MovementModuleManagerCount: " + movementModules.Count);
        foreach (IMovementModule movementModule in movementModules)
        {
            Debug.Log("MovementModule: " + movementModule.GetType().ToString());
            movementModule.Run(rb, forceTarget, forceAllocated, timeOffset);
        }
    }
}
