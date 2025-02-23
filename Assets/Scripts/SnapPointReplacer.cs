using UnityEditor;
using UnityEngine;

[ExecuteInEditMode]
public class SnapPointReplacer : MonoBehaviour
{
    [SerializeField] private GameObject snapPointPrefab;

    [SerializeField] private bool EasyButton;
    private void Update()
    {
        if (EasyButton)
        {
            EasyButton = false;
            ReplaceSnapPoints();
        }
    }

    private void ReplaceSnapPoints()
    {
        JointPointSnap jointPointSnaps = GetComponent<JointPointSnap>();
        if (jointPointSnaps == null)
        {
            Debug.LogError("No JointPointSnap component found in parent object");
            return;
        }

        SnapPoint[] snapPoints = GetComponentsInChildren<SnapPoint>();
        foreach (SnapPoint snapPoint in snapPoints)
        {
            GameObject newSnapPoint = PrefabUtility.InstantiatePrefab(snapPointPrefab) as GameObject;
            newSnapPoint.transform.position = snapPoint.transform.position;
            newSnapPoint.transform.rotation = snapPoint.transform.rotation;
            newSnapPoint.transform.SetParent(snapPoint.transform.parent);
            newSnapPoint.name = snapPoint.name;

            if (jointPointSnaps.GetMainSnapPoints.IndexOf(snapPoint) != -1)
            {
                jointPointSnaps.GetMainSnapPoints[jointPointSnaps.GetMainSnapPoints.IndexOf(snapPoint)] = newSnapPoint.GetComponent<SnapPoint>();
            }
            else if (jointPointSnaps.GetSondarySnapPoints.IndexOf(snapPoint) != -1)
            {
                jointPointSnaps.GetSondarySnapPoints[jointPointSnaps.GetSondarySnapPoints.IndexOf(snapPoint)] = newSnapPoint.GetComponent<SnapPoint>();
            }

            DestroyImmediate(snapPoint.gameObject);
        }
    }
}
