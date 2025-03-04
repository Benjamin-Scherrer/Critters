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

    [Header("StartupTime")]
    [SerializeField] private float hardStartupTime = 1f;
    [SerializeField] private float softStartupTime = 4f;
    private float timeElapsed = 0f;

    //PUBLIC
    public void Exlpode(Vector3 explosionOrigin, float explosionForce)
    {
        Vector3 explosionDirection = (forceTarget.position - explosionOrigin).normalized;
        rb.AddForceAtPosition(explosionDirection * explosionForce, forceTarget.position, ForceMode.Impulse);
    }

    public void ResetStartupTime()
    {
        timeElapsed = 0f;
    }

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
        //Debug.Log("TimeElapsed:" + timeElapsed);

        if (timeElapsed < hardStartupTime)
        {
            timeElapsed += Time.deltaTime;
            return;
        }
        if (timeElapsed < hardStartupTime + softStartupTime)
        {
            timeElapsed = Mathf.Min(timeElapsed + Time.fixedDeltaTime, hardStartupTime + softStartupTime);
        }

        float startUpBasedForce = ((timeElapsed - hardStartupTime) / softStartupTime);
        foreach (IMovementModule movementModule in movementModules)
        {
            movementModule.Run(rb, forceTarget, forceAllocated * startUpBasedForce, timeOffset);
        }
    }
}
