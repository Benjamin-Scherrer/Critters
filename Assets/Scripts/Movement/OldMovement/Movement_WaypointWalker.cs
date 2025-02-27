using UnityEngine;

public class Movement_WaypointWalker : Movement_Abstract
{
    [Header("Variables")]
    [SerializeField] [Range(0f, 1f)] private float movementToRotationRatio = 0.5f;
    [SerializeField] [Range(0f, 1f)] private float forwardDirectionToDirectionToWayPointRatio = 0.5f;
    [SerializeField] [Range(0f, 5f)] private float minDistanceToWayPoint = 2f;
    
    [Header("References")]
    [SerializeField] private Transform pointForceTarget;
    [SerializeField] private Transform wayPoint;

    //[Header("Features")]
    //[SerializeField] private bool snapToSurface;

    //easiest solution is to make point force target turn away frrom all limb attached to the head.

    protected override Vector3 LocalDirectionOscilating
    {
        get
        {
            return localDirection ? pointForceTarget.TransformDirection(DirectionOscilating) : DirectionOscilating;
        }
    }

    private Transform GetWaypoint
    {
        get
        {
            if (wayPoint == null)
            {
                wayPoint = GameObject.Find("Waypoint").transform;
            }
            return wayPoint;
        }
    }

    //private void Start()
    //{
    //    if (snapToSurface)
    //    {
    //        RaycastHit hit;
    //        if (Physics.Raycast(pointForceTarget.position, transform.position - pointForceTarget.position, out hit))
    //        {
    //            pointForceTarget.position = hit.point;
    //        }
    //    }
    //}

    private void FixedUpdate()
    {
        float forwardForce = ForceOscilating * movementToRotationRatio;
        float turnForce = ForceOscilating * (1 - movementToRotationRatio);

        MoveForwards(forwardForce);
        TurnToWayPoint(turnForce);


        if(Vector3.Distance(transform.position, GetWaypoint.position) < minDistanceToWayPoint)
        {
            wayPoint = WayPointManager.Instance.GetWayPoint(GetWaypoint);
        }
    }


    private void MoveForwards(float force)
    {
        //Mix directions to waypoint and localDirectionOscilating
        Vector3 directionToWaypoint = (GetWaypoint.position - transform.position).normalized;
        Vector3 directionMix = Vector3.Lerp(directionToWaypoint, LocalDirectionOscilating.normalized, forwardDirectionToDirectionToWayPointRatio).normalized;

        rb.AddForceAtPosition(directionMix * force, pointForceTarget.position, ForceMode.Impulse);
    }

    private void TurnToWayPoint(float force)
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

        rb.AddTorque(force * Mathf.Sign(angle) * axis, ForceMode.Impulse);
    }


    private void OnDrawGizmos()
    {
        if (!isActiveAndEnabled) return;

        //POINTFORCE
        Vector3 directionToWaypoint = (GetWaypoint.position - transform.position).normalized;
        Vector3 directionMix = Vector3.Lerp(directionToWaypoint, LocalDirectionOscilating.normalized, forwardDirectionToDirectionToWayPointRatio).normalized;
        Vector3 offset1 = directionMix.normalized * GizmoSize * 2f;

        Gizmos.color = Color.green;
        Gizmos.DrawLine(pointForceTarget.position, pointForceTarget.position + offset1);


        //WAYPOINT
        //Vector3 directionToWaypoint = (GetWaypoint.position - transform.position).normalized;
        Quaternion targetRotation = Quaternion.LookRotation(directionToWaypoint);
        Quaternion currentRotation = transform.rotation;
        Quaternion rotationDifference = targetRotation * Quaternion.Inverse(currentRotation);
        rotationDifference.ToAngleAxis(out float angle, out Vector3 axis);
        axis = (axis.normalized + LocalDirectionOscilating.normalized).normalized;

        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + directionToWaypoint * GizmoSize);

        Vector3 offset2 = 0.5f * GizmoSize * axis * Mathf.Sign(angle);
        Gizmos.DrawLine(transform.position - offset2, transform.position + offset2);
    }
}
