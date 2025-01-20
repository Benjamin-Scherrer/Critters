using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement_PointForce : Movement_Abstract
{
    [Header("References")]
    [SerializeField] private Transform target;

    [Header("Features")]
    [SerializeField] private bool snapToSurfave;

    protected override Vector3 LocalDirection
    {
        get
        {
            return target.TransformDirection(direction);
        }
    }

    private void Start()
    {
        if (snapToSurfave)
        {
            RaycastHit hit;
            if (Physics.Raycast(target.position, transform.position-target.position, out hit))
            {
                target.position = hit.point;
            }
        }
    }

    private void FixedUpdate()
    {
        rb.AddForceAtPosition(LocalDirectionOscilating.normalized * ForceOscilating, target.position, ForceMode.Force);
    }

    private void OnDrawGizmos()
    {
        Vector3 offset = LocalDirectionOscilating.normalized * GizmoSize;

        Gizmos.color = Color.red;
        Gizmos.DrawLine(target.position, target.position + offset);
    }
}
