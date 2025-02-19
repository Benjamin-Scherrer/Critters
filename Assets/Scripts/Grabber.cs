using UnityEngine;

public class Grabber : MonoBehaviour {

    [Range(0.2f,1f)]
    [SerializeField] private float grabSlowMotion = 0.8f;

    [Header("Joint Settings")][Space]
    [SerializeField] private float posSpring = 40f;
    [SerializeField] private float posDamp = 10f;
    [SerializeField] private float rotSpring = 20f;
    [SerializeField] private float rotDamp = 5f;

    private Vector3 position;
    private Vector3 worldPosition;
    private Rigidbody rb;
    private MeshRenderer meshRenderer;
    private GameObject snappedGeobody;
    private ConfigurableJoint joint;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        meshRenderer = GetComponent<MeshRenderer>();
        meshRenderer.enabled = false;
    }

    private void Update() {

        position = new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.WorldToScreenPoint(transform.position).z);
        worldPosition = Camera.main.ScreenToWorldPoint(position);

        if (Input.GetMouseButton(0)) {
            if(snappedGeobody == null) {
                RaycastHit hit = CastRay();

                if(hit.collider != null) {
                    if (!hit.collider.CompareTag("Geobody")) {
                        return;
                    }

                    snappedGeobody = hit.collider.gameObject;
                    transform.position = snappedGeobody.transform.position;
                    joint = snappedGeobody.AddComponent<ConfigurableJoint>();
                    joint.connectedBody = rb;
                    joint.anchor = Vector3.zero;
                    joint.rotationDriveMode = RotationDriveMode.Slerp;
                    var posDrive = new JointDrive { positionSpring = posSpring, positionDamper = posDamp, maximumForce = Mathf.Infinity };
                    var rotDrive = new JointDrive { positionSpring = rotSpring, positionDamper = rotDamp, maximumForce = Mathf.Infinity };
                    joint.xDrive = posDrive;
                    joint.yDrive = posDrive;
                    joint.zDrive = posDrive;
                    joint.slerpDrive = rotDrive;
                    meshRenderer.enabled = true;
                    Cursor.visible = false;
                    Time.timeScale = grabSlowMotion;
                }
            } else {
                transform.position = new Vector3(worldPosition.x, 1f, worldPosition.z);
            }
        }

        else if (snappedGeobody != null)
        {
            transform.position = new Vector3(worldPosition.x, 0.5f, worldPosition.z);
            Destroy(joint);
            snappedGeobody = null;
            meshRenderer.enabled = false;
            Cursor.visible = true;
            Time.timeScale = 1f;
        }
    }

    private RaycastHit CastRay() {
        Vector3 screenMousePosFar = new Vector3(
            Input.mousePosition.x,
            Input.mousePosition.y,
            Camera.main.farClipPlane);
        Vector3 screenMousePosNear = new Vector3(
            Input.mousePosition.x,
            Input.mousePosition.y,
            Camera.main.nearClipPlane);
        Vector3 worldMousePosFar = Camera.main.ScreenToWorldPoint(screenMousePosFar);
        Vector3 worldMousePosNear = Camera.main.ScreenToWorldPoint(screenMousePosNear);
        RaycastHit hit;
        Physics.Raycast(worldMousePosNear, worldMousePosFar - worldMousePosNear, out hit);

        return hit;
    }
}
