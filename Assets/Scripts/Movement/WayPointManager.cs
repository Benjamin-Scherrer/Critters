using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WayPointManager : MonoBehaviour
{
    public static WayPointManager Instance;

    [SerializeField] private List<Transform> wayPoints;

    private void Awake()
    {
        Instance = this;
    }

    public Transform GetWayPoint(Transform previousWayPoint)
    {
        if(wayPoints.Count == 0) throw new System.Exception("WayPointManager has no waypoints assigned");

        int randomIndex = Random.Range(0, wayPoints.Count);
        if(previousWayPoint != null && wayPoints[randomIndex] == previousWayPoint)
        {
            randomIndex = (randomIndex + 1) % wayPoints.Count;
        }
        return wayPoints[randomIndex];
    }
}
