using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WayPointManager : MonoBehaviour
{
    public static WayPointManager Instance;

    [SerializeField] private float IgnoreCloserPercentageCoef = 0.5f;

    [SerializeField] private List<Transform> wayPoints;


    private float distanceTreshhold = 0;

    //PUBLIC
    public Transform GetWayPoint(Transform previousWayPoint)
    {
        if (wayPoints.Count == 0) throw new System.Exception("WayPointManager has no waypoints assigned");

        int randomIndex = Random.Range(0, wayPoints.Count);
        for (int i = randomIndex, steps = 0; steps < wayPoints.Count; i++, steps++)
        {
            i = i % wayPoints.Count;
            if (Vector3.Distance(wayPoints[i].position, previousWayPoint.position) < distanceTreshhold) continue;
            if (previousWayPoint != null && wayPoints[i] == previousWayPoint) continue;
            return wayPoints[i];
        }
        return wayPoints[(randomIndex + 1) % wayPoints.Count];
        //throw new System.Exception("No available waypoints");
    }

    //PRIVATE
    private void Awake()
    {
        Instance = this;



        CalculateDistancetreshhold();
    }


    private void CalculateDistancetreshhold()
    {
        float largestDistance = 0;

        for (int i = 0; i < wayPoints.Count; i++)
        {
            for (int j = 0; i < wayPoints.Count; i++)
            {
                if(i == j) continue;
                float distance = Vector3.Distance(wayPoints[j].position, wayPoints[i].position);
                if (distance < largestDistance) continue;
                largestDistance = distance;
            }
        }

        //Debug.Log("Larget Distance: " + largestDistance);
        distanceTreshhold = largestDistance * IgnoreCloserPercentageCoef;
        //Debug.Log("Distance Treshhold:" + distanceTreshhold);
    }
}
