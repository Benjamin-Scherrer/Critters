using UnityEngine;

public class SnapPoint : MonoBehaviour
{
    public bool connected = false;

    private void OnDrawGizmosSelected()
    {
        transform.parent.GetComponent<JointPointSnap>().DrawSphere();
    }
}
