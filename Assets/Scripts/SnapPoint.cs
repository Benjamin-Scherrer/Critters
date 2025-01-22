using UnityEngine;

public class SnapPoint : MonoBehaviour
{
    private void OnDrawGizmosSelected()
    {
        transform.parent.GetComponent<JointPointSnap>().DrawSphere();
    }
}
