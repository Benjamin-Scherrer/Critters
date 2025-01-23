using UnityEngine;

public class SnapPoint : MonoBehaviour
{
    public bool connected = false;
    public Collider coll;

    void Start()
    {
        coll = GetComponent<Collider>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (connected) return;
        transform.parent.GetComponent<JointPointSnap>().Snap(other, this);
    }
}
