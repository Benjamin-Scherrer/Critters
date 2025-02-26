using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Movement_FaceToWaypoint : Movement_Abstract
{
    [Header("References")]
    [SerializeField] private Transform wayPoint;


    private Transform GetWaypoint
    {
        get
        {
            if (wayPoint == null)
            {
                wayPoint = WayPointManager.Instance.GetWayPoint(null);
            }
            return wayPoint;
        }
    }

    private void FixedUpdate()
    {
        TurnTowardsWaypoint();
    }

    private void TurnTowardsWaypoint()
    {
        // Calculate direction to the waypoint
        Vector3 directionToWaypoint = (GetWaypoint.position - transform.position).normalized;

        // Calculate the rotation needed to face the waypoint
        Quaternion targetRotation = Quaternion.LookRotation(directionToWaypoint);
        Quaternion currentRotation = transform.rotation;

        // Calculate the difference in rotation
        Quaternion rotationDifference = targetRotation * Quaternion.Inverse(currentRotation);

        // Convert the rotation difference to an angle-axis representation
        rotationDifference.ToAngleAxis(out float angle, out Vector3 axis);

        // Apply torque to rotate towards the waypoint with localDirectionOscilating as an offset
        axis = (axis.normalized + LocalDirectionOscilating.normalized).normalized;

        rb.AddTorque(ForceOscilating * Mathf.Sign(angle) * axis, ForceMode.Impulse);
    }


    private void OnDrawGizmos()
    {
        Vector3 directionToWaypoint = (GetWaypoint.position - transform.position).normalized;
        Quaternion targetRotation = Quaternion.LookRotation(directionToWaypoint);
        Quaternion currentRotation = transform.rotation;
        Quaternion rotationDifference = targetRotation * Quaternion.Inverse(currentRotation);
        rotationDifference.ToAngleAxis(out float angle, out Vector3 axis);
        axis = (axis.normalized + LocalDirectionOscilating.normalized).normalized;


        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + directionToWaypoint * GizmoSize);


        Vector3 offset = 0.5f * GizmoSize * axis * Mathf.Sign(angle);
        Gizmos.DrawLine(transform.position - offset, transform.position + offset);
    }
}
