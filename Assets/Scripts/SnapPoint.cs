using UnityEngine;

public class SnapPoint : MonoBehaviour
{
    [HideInInspector] public Collider coll;

    void Start()
    {
        coll = GetComponent<Collider>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("SnapPoint"))
        {
            if (!coll.enabled) return;
            transform.parent.GetComponent<JointPointSnap>().Snap(other, this);
        }
    }
}
