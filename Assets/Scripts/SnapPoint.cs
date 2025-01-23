using UnityEngine;

public class SnapPoint : MonoBehaviour
{
    public bool connected = false;
    private Collider coll;

    void Start()
    {
        coll = GetComponent<Collider>();
    }

    void Update()
    {
        if (connected)
        {
            coll.enabled = false;
        }

        else coll.enabled = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (connected) return;
        transform.parent.GetComponent<JointPointSnap>().Snap(other, this);
    }
}
