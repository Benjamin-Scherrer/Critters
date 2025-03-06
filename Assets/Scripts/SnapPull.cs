using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnapPull : MonoBehaviour
{
    [SerializeField] private float pullForce = 10;

    void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Magnet"))
        {
            if (other.transform.parent.parent.TryGetComponent<Rigidbody>(out Rigidbody otherRigidbody))
            {
                otherRigidbody.AddForce((transform.position - other.transform.position).normalized * pullForce);
            }
        }
    }
}
