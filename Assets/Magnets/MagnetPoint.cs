using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagnetPoint : MonoBehaviour
{
    public GameObject parentBody; // Reference to the main body this magnet belongs to
    public bool isConnected = false; // Prevents creating multiple joints

    private void OnTriggerEnter(Collider other)
    {
        // Check if the colliding object is another magnet point
        MagnetPoint otherMagnetPoint = other.GetComponent<MagnetPoint>();
        if (otherMagnetPoint != null && !isConnected && !otherMagnetPoint.isConnected)
        {
            // Mark both magnet points as connected
            isConnected = true;
            otherMagnetPoint.isConnected = true;

            // Create the joint between the parent bodies at the collision point
            CreateJointWith(otherMagnetPoint, other.ClosestPoint(transform.position));
        }
    }

    private void CreateJointWith(MagnetPoint otherMagnetPoint, Vector3 collisionPoint)
    {
        // Get the rigidbodies of both parent objects
        Rigidbody thisBody = parentBody.GetComponent<Rigidbody>();
        Rigidbody otherBody = otherMagnetPoint.parentBody.GetComponent<Rigidbody>();

        if (thisBody != null && otherBody != null)
        {
            // Create a CharacterJoint on this parent body
            CharacterJoint joint = parentBody.AddComponent<CharacterJoint>();
            joint.connectedBody = otherBody;

            // Calculate the local anchor positions for the collision point
            joint.anchor = parentBody.transform.InverseTransformPoint(collisionPoint);
            joint.connectedAnchor = otherMagnetPoint.parentBody.transform.InverseTransformPoint(collisionPoint);

            // Optional: Configure joint limits
            SoftJointLimit limit = new SoftJointLimit();
            limit.limit = 45f; // Example limit for flexibility
            joint.swing1Limit = limit;
            joint.swing2Limit = limit;
            

            Debug.Log($"Joint created between {parentBody.name} and {otherMagnetPoint.parentBody.name} at collision point: {collisionPoint}");
        }
        else
        {
            Debug.LogWarning("One or both parent bodies are missing a Rigidbody.");
        }
    }
}