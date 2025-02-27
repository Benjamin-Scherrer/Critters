using extOSC;
using UnityEngine;

public class Grabber : MonoBehaviour
{

    [Header("OSC Settings")]
    [SerializeField] private OSCReceiver Receiver;
    [SerializeField] private string Address = "/ipad";

    [Header("Grabber Settings")]
    [Range(0.2f, 1f)]
    [SerializeField] private float grabSlowMotion = 0.8f;
    [SerializeField] private float grabLift = 2f;
    [SerializeField] private float floatHeight = 0.25f;
    [SerializeField] private float grabRadius = 1f;

    [Header("Joint Settings")]
    [SerializeField] private float posSpring = 40f;
    [SerializeField] private float posDamp = 10f;
    [SerializeField] private float rotSpring = 20f;
    [SerializeField] private float rotDamp = 5f;

    private Vector3 oscPosition = Vector3.zero;
    private bool oscDown = false;
    private Vector3 worldPosition;
    private Rigidbody rb;
    private MeshRenderer meshRenderer;
    private GameObject snappedGeobody;
    private ConfigurableJoint joint;
    private Camera mainCam;

    private LayerMask geobodyLayer;
    private LayerMask backgroundLayer;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        meshRenderer = GetComponent<MeshRenderer>();
        meshRenderer.enabled = false;
        geobodyLayer = LayerMask.GetMask("Geobody");
        backgroundLayer = LayerMask.GetMask("Background");
    }

    private void Start()
    {
        mainCam = Camera.main;
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
        RaycastHit geobodyHit;
        RaycastHit cursorHit;
        Vector3 rayOrigin = mainCam.ScreenToWorldPoint(new Vector3(inputPosition.x, inputPosition.y, mainCam.nearClipPlane));
        Vector3 rayDirection = mainCam.ScreenToWorldPoint(new Vector3(0f, 0f, mainCam.farClipPlane - mainCam.nearClipPlane));
        Physics.Raycast(rayOrigin, rayDirection, out cursorHit, Mathf.Infinity, backgroundLayer);
        Physics.SphereCast(rayOrigin, grabRadius, rayDirection, out geobodyHit, Mathf.Infinity, geobodyLayer);
        worldPosition = cursorHit.point;
        rb.position = worldPosition - rayDirection.normalized * floatHeight;

        if (down)
        {
            if (snappedGeobody == null)
            {
                if (cursorHit.collider == null) return;

                meshRenderer.enabled = true;
                Cursor.visible = false;
                Time.timeScale = grabSlowMotion;

                if (geobodyHit.collider == null) return;

                snappedGeobody = geobodyHit.collider.gameObject;
                joint = snappedGeobody.AddComponent<ConfigurableJoint>();
                joint.autoConfigureConnectedAnchor = false;
                joint.connectedBody = rb;
                joint.anchor = Vector3.zero;
                joint.connectedAnchor = snappedGeobody.transform.localPosition - transform.localPosition;
                joint.rotationDriveMode = RotationDriveMode.Slerp;
                var posDrive = new JointDrive { positionSpring = posSpring, positionDamper = posDamp, maximumForce = Mathf.Infinity };
                var rotDrive = new JointDrive { positionSpring = rotSpring, positionDamper = rotDamp, maximumForce = Mathf.Infinity };
                joint.xDrive = posDrive;
                joint.yDrive = posDrive;
                joint.zDrive = posDrive;
                joint.slerpDrive = rotDrive;
            }
            else
            {
                rb.position = worldPosition - rayDirection.normalized * (floatHeight + grabLift);
            }
        }
        else
        {
            if (snappedGeobody != null)
            {
                Destroy(joint);
                snappedGeobody = null;
            }

            meshRenderer.enabled = false;
            Cursor.visible = true;
            Time.timeScale = 1f;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(worldPosition, grabRadius);
    }
}
