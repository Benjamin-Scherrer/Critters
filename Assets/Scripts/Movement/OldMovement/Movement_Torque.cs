using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Movement_Torque : Movement_Abstract
{
    private void FixedUpdate()
    {
        rb.AddTorque(LocalDirectionOscilating.normalized * ForceOscilating, ForceMode.Impulse);
    }

    private void OnDrawGizmos()
    {
        Vector3 offset = 0.5f * GizmoSize * LocalDirectionOscilating.normalized;

        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position - offset, transform.position + offset);
    }
}
