using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Movement_Torgue : Movement_Abstract
{
    private void FixedUpdate()
    {
        rb.AddTorque(LocalDirectionOscilating.normalized * ForceOscilating, ForceMode.Force);
    }

    private void OnDrawGizmos()
    {
        Vector3 offset = LocalDirectionOscilating.normalized * GizmoSize * 0.5f;

        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position - offset, transform.position + offset);
    }
}
