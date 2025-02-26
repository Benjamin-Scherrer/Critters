using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WayPointManager : MonoBehaviour
{
    public static WayPointManager Instance;

    [SerializeField] private float IgnoreCloserPercentageCoef = 0.5f;

    [SerializeField] private List<Transform> wayPoints;


    private float distanceTreshhold = 0;

    private void Awake()
    {
        Instance = this;


        float largestDistance = 0;

        for (int i = 1; i < wayPoints.Count; i++)
        {
            Transform wayPoint = wayPoints[i];
            float distance = Vector3.Distance(wayPoint.position, wayPoints[0].position);
            if(distance > largestDistance)
            {
                largestDistance = distance;
            }
        }
        distanceTreshhold = largestDistance * IgnoreCloserPercentageCoef;
    }

    public Transform GetWayPoint(Transform previousWayPoint)
    {
        if(wayPoints.Count == 0) throw new System.Exception("WayPointManager has no waypoints assigned");

        int randomIndex = Random.Range(0, wayPoints.Count);
        for(int i = randomIndex, steps = 0; steps < wayPoints.Count; i++, steps++)
        {
            i = i % wayPoints.Count;
            if (Vector3.Distance(wayPoints[i].position, previousWayPoint.position) < distanceTreshhold) continue;
            if (previousWayPoint != null && wayPoints[randomIndex] == previousWayPoint) continue;
            return wayPoints[i];
        }
        throw new System.Exception("No available waypoints");
    }
}
