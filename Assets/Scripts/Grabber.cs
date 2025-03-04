using extOSC;
using FMOD;
using UnityEngine;

public class Grabber : MonoBehaviour
{
    public static Grabber Instance;

    public bool isGrabbing = false;
    private bool playedEffect = false;

    [Header("OSC Settings")]
    [SerializeField] private OSCReceiver Receiver;
    [SerializeField] private string Address = "/ipad";

    [Header("Grabber Settings")]
    [Range(0.2f, 1f)]
    [SerializeField] private float grabSlowMotion = 0.8f;
    [SerializeField] private float grabLift = 2f;
    [SerializeField] private float floatHeight = 0.25f;
    [SerializeField] private float grabRadius = 1f;
    [SerializeField] private ParticleSystem cursorWave;


    [Header("Joint Settings")]
    [SerializeField] private ConfigurableJoint jointPrefab;

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
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }

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
        Cursor.visible = false;
    }

    private void Update()
    {
        if (Input.GetMouseButton(0))
        {
            Drag(Input.mousePosition, Input.GetMouseButton(0));
        }
        else
        {
            Drag(oscPosition, oscDown);
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
        isGrabbing = down;

        if (down && !playedEffect)
        {
            cursorWave.Play();
            playedEffect = true;
        }
        else if (!down && playedEffect)
        {
            playedEffect = false;
        }

        inputPosition = new Vector3(Mathf.Clamp(inputPosition.x, 0f, Screen.width), Mathf.Clamp(inputPosition.y, 0f, Screen.height), 0f);
        RaycastHit geobodyHit;
        RaycastHit cursorHit;
        Vector3 rayOrigin = mainCam.ScreenToWorldPoint(new Vector3(inputPosition.x, inputPosition.y, mainCam.nearClipPlane));
        Vector3 rayDestination = mainCam.ScreenToWorldPoint(new Vector3(inputPosition.x, inputPosition.y, mainCam.farClipPlane));
        Vector3 rayDirection = rayDestination - rayOrigin;
        Physics.Raycast(rayOrigin, rayDirection, out cursorHit, Mathf.Infinity, backgroundLayer);
        Physics.SphereCast(rayOrigin, grabRadius, rayDirection, out geobodyHit, Mathf.Infinity, geobodyLayer);
        worldPosition = cursorHit.point;
        transform.position = worldPosition - rayDirection.normalized * floatHeight;

        if (down)
        {
            if (snappedGeobody == null)
            {
                if (cursorHit.collider == null) return;

                meshRenderer.enabled = true;
                //Cursor.visible = false;
                Time.timeScale = grabSlowMotion;

                if (geobodyHit.collider == null) return;

                snappedGeobody = geobodyHit.collider.gameObject;
                joint = SetJointPrefabSettings(snappedGeobody.AddComponent<ConfigurableJoint>());
                joint.connectedBody = rb;
                joint.anchor = Vector3.zero;
                joint.connectedAnchor = snappedGeobody.transform.localPosition - transform.localPosition;
            }
            else
            {
                transform.position = worldPosition - rayDirection.normalized * (floatHeight + grabLift);
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
            //Cursor.visible = true;
            Time.timeScale = 1f;
        }
    }

    private ConfigurableJoint SetJointPrefabSettings(ConfigurableJoint joint)
    {
        ////Useless assignement bc its overwriten later,
        //joint.connectedBody = jointPrefab.connectedBody;
        //joint.connectedArticulationBody = jointPrefab.connectedArticulationBody;

        ////Useless assignement bc its overwriten later,
        //joint.anchor = jointPrefab.anchor;

        //Special settings for that
        //joint.axis = jointPrefab.axis;

        //yes but needs to be always set to false, condiering to hardcode this.
        joint.autoConfigureConnectedAnchor = jointPrefab.autoConfigureConnectedAnchor;
        //joint.autoConfigureConnectedAnchor = false;

        ////Useless assignement bc its overwriten later,
        //joint.connectedAnchor = jointPrefab.connectedAnchor;

        //special settings for that
        //joint.secondaryAxis = jointPrefab.secondaryAxis;

        //probably set to limited
        joint.xMotion = jointPrefab.xMotion;
        joint.yMotion = jointPrefab.yMotion;
        joint.zMotion = jointPrefab.zMotion;

        //probably set to free
        joint.angularXMotion = jointPrefab.angularXMotion;
        joint.angularYMotion = jointPrefab.angularYMotion;
        joint.angularZMotion = jointPrefab.angularZMotion;

        //here go settings
        joint.linearLimitSpring = jointPrefab.linearLimitSpring;
        joint.linearLimit = jointPrefab.linearLimit;

        ////nothing bc of free //not free now
        joint.angularXLimitSpring = jointPrefab.angularXLimitSpring;
        joint.lowAngularXLimit = jointPrefab.lowAngularXLimit;
        joint.highAngularXLimit = jointPrefab.highAngularXLimit;
        joint.angularYZLimitSpring = jointPrefab.angularYZLimitSpring;
        joint.angularYLimit = jointPrefab.angularYLimit;
        joint.angularZLimit = jointPrefab.angularZLimit;

        ////will be vector3.zero to get to stable position -> So no assignement needed
        //joint.targetPosition = jointPrefab.targetPosition;
        //joint.targetVelocity = jointPrefab.targetVelocity;

        joint.xDrive = jointPrefab.xDrive;
        joint.yDrive = jointPrefab.yDrive;
        joint.zDrive = jointPrefab.zDrive;

        ////will be vector3.zero to get to stable position -> So no assignement needed
        //joint.targetRotation = jointPrefab.targetRotation;
        //joint.targetAngularVelocity = jointPrefab.targetAngularVelocity;

        //probably slerp drive
        joint.rotationDriveMode = jointPrefab.rotationDriveMode;

        ////nothing bc of slerp drive
        //joint.angularXDrive = jointPrefab.angularXDrive;
        //joint.angularYZDrive = jointPrefab.angularYZDrive;

        //filled with slerp drive data
        joint.slerpDrive = jointPrefab.slerpDrive;

        ////probably nothing bc of none
        //joint.projectionMode = jointPrefab.projectionMode;
        //joint.projectionDistance = jointPrefab.projectionDistance;
        //joint.projectionAngle = jointPrefab.projectionAngle;

        //probably none bc we need the anchors relative to the geobodies    
        joint.configuredInWorldSpace = jointPrefab.configuredInWorldSpace;

        //suss mogus maybe swap every update lololol
        joint.swapBodies = jointPrefab.swapBodies;

        //RN none but eventually maybe to make the geobodies break apart
        joint.breakForce = jointPrefab.breakForce;
        joint.breakTorque = jointPrefab.breakTorque;

        //probably yes
        joint.enableCollision = jointPrefab.enableCollision;
        joint.enablePreprocessing = jointPrefab.enablePreprocessing;

        //idk what this does.
        joint.massScale = jointPrefab.massScale;
        joint.connectedMassScale = jointPrefab.connectedMassScale;

        return joint;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(worldPosition, grabRadius);
    }
}
