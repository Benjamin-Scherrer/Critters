using UnityEditor.UIElements;
using UnityEditorInternal;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Looper : MonoBehaviour
{
    [SerializeField] private string Tag;

    private BoxCollider Collider;
    private Vector3 boundary;

    private void Start()
    {
        Collider = GetComponent<BoxCollider>();

        if (Collider.isTrigger == false)
        {
            //Debug.LogWarning("Collider is not a trigger, setting it to trigger");
            Collider.isTrigger = true;
        }

        // Set the boundary to the size of the collider's bounds
        boundary = Collider.size;
        //Debug.Log("Boundary: " + boundary);
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag(Tag)) return;

        Vector3 newPosition = transform.InverseTransformPoint(other.transform.position);
        //Debug.Log("PreviousPosition: " + newPosition);

        newPosition.x = WrapPosition(newPosition.x, boundary.x);
        newPosition.y = WrapPosition(newPosition.y, boundary.y);
        newPosition.z = WrapPosition(newPosition.z, boundary.z);

        newPosition = transform.TransformPoint(newPosition);
        //Debug.Log("NewPosition: " + newPosition);

        if(newPosition == other.transform.position)
        {
            Debug.Log("Object didn't loop despite being outside of bounds...");
            return;
        }

        other.attachedRigidbody.isKinematic = true; // Disable physics interactions
        other.transform.position = newPosition;
        other.attachedRigidbody.isKinematic = false; // Re-enable physics
       // Debug.Log("Object has been teleported");
    }

    private float WrapPosition(float position, float boundary)
    {
        //Debug.Log("Initial Position: " + position + ", boundary:" + boundary);
        if (position >= boundary * 0.5f)
        {
            //Debug.Log("Overflow: " + (position - boundary));
            return position - boundary;
        }
        else if (position <= -boundary * 0.5f)
        {
            //Debug.Log("Underflow: " + (position + boundary));
            return  position + boundary;
        }
        //Debug.Log("No Overflow/Underflow");
        return position;
    }
}
