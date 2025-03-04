using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConnectionPointsDisplay : MonoBehaviour
{
    [SerializeField] private GameObject targetObject; // Assign this in the inspector

    private void Update()
    {
        if (targetObject != null)
        {
            targetObject.SetActive(Grabber.Instance.isGrabbing);
        }
    }
}
