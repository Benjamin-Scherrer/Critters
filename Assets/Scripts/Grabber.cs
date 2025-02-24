using extOSC;
using UnityEngine;

public class Grabber : MonoBehaviour {

    [Header("OSC Settings")][Space]
    [SerializeField] private OSCReceiver Receiver;
    [SerializeField] private string Address = "/ipad";

    [Header("Grabber Settings")][Space]
    [Range(0.2f,1f)]
    [SerializeField] private float grabSlowMotion = 0.8f;
    [Range(0f, 5f)]
    [SerializeField] private float grabLift = 2f;
    [SerializeField] private float floatHeight = 0.25f;

    [Header("Joint Settings")][Space]
    [SerializeField] private float posSpring = 40f;
    [SerializeField] private float posDamp = 10f;
    [SerializeField] private float rotSpring = 20f;
    [SerializeField] private float rotDamp = 5f;

    private Vector3 oscPosition = Vector3.zero;
    private bool oscDown = false;
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
        Receiver.Bind(Address, MapValues);
    }

    private void Update()
    {
        if (oscDown)
        {
            Drag(oscPosition, oscDown);
        }
        else
        {
            Drag(Input.mousePosition, Input.GetMouseButton(0));
        }
    }

    private void MapValues(OSCMessage message)
    {
        var values = message.FindValues(OSCValueType.Float, OSCValueType.True, OSCValueType.False);
        oscPosition = new Vector3((float)values[0].FloatValue * Screen.width, (float)values[1].FloatValue * Screen.height, 0);
        oscDown = values[2].BoolValue;
    }

    private void Drag(Vector3 inputPosition, bool down)
    {
        position = new Vector3(inputPosition.x, inputPosition.y, Camera.main.WorldToScreenPoint(transform.position).z);
        worldPosition = Camera.main.ScreenToWorldPoint(position);

        if (down)
        {
            if (snappedGeobody == null)
            {
                RaycastHit hit = CastRay(inputPosition);

                if (hit.collider != null)
                {
                    meshRenderer.enabled = true;
                    Cursor.visible = false;
                    Time.timeScale = grabSlowMotion;
                    transform.position = new Vector3(worldPosition.x, floatHeight, worldPosition.z);

                    if (!hit.collider.CompareTag("Geobody"))
                    {
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
                }
            }
            else
            {
                transform.position = new Vector3(worldPosition.x, floatHeight + grabLift, worldPosition.z);
            }
        }

        else 
        {
            if (snappedGeobody != null)
            {
                transform.position = new Vector3(worldPosition.x, floatHeight, worldPosition.z);
                Destroy(joint);
                snappedGeobody = null;
            }

            meshRenderer.enabled = false;
            Cursor.visible = true;
            Time.timeScale = 1f;
        }
    }

    private RaycastHit CastRay(Vector3 inputPosition) {
        Vector3 screenMousePosFar = new Vector3(
            inputPosition.x,
            inputPosition.y,
            Camera.main.farClipPlane);
        Vector3 screenMousePosNear = new Vector3(
            inputPosition.x,
            inputPosition.y,
            Camera.main.nearClipPlane);
        Vector3 worldMousePosFar = Camera.main.ScreenToWorldPoint(screenMousePosFar);
        Vector3 worldMousePosNear = Camera.main.ScreenToWorldPoint(screenMousePosNear);
        RaycastHit hit;
        Physics.Raycast(worldMousePosNear, worldMousePosFar - worldMousePosNear, out hit);

        return hit;
    }
}
