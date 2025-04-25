using extOSC;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputAdapter : MonoBehaviour
{
    [Header("OSC Settings")]
    [SerializeField] private OSCReceiver Receiver;
    [SerializeField] private string Address = "/ipad";

    [Header("Input Settings")]
    [SerializeField] private float inputTimeout = 10;

    [Header("Debug")]
    [SerializeField] private float currentInputTimer;

    public bool idle = false;

    public Vector3 oscPosition = Vector3.zero;
    public bool oscDown = false;

    public static InputAdapter Instance;

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
    }

    void Start()
    {
        Receiver.Bind(Address, MapValues);
    }

    void Update()
    {
        if (oscDown)
        {
            currentInputTimer = inputTimeout;
            idle = false;
        }

        if (currentInputTimer > 0)
        {
            currentInputTimer -= Time.deltaTime;
            idle = false;
        }
        else
        {
            idle = true;
        }
    }


    private void MapValues(OSCMessage message)
    {
        var values = message.FindValues(OSCValueType.Float, OSCValueType.True, OSCValueType.False);
        oscPosition = new Vector3((float)values[0].FloatValue * Screen.width, (float)values[1].FloatValue * Screen.height, 0);
        oscDown = values[2].BoolValue;
    }
}
