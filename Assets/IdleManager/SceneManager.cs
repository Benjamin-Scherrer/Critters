using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class SceneManager : MonoBehaviour
{
    [SerializeField] private  List<CinemachineVirtualCamera> virtualCameras; // List of virtual cameras
    [SerializeField] private float idleTime = 10f; // Time before switching cameras
    [SerializeField] private float currentTimer;
    [SerializeField] private CinemachineVirtualCamera currentCamera;

    void Start()
    {
        if (virtualCameras.Count == 0) return;

        currentCamera = virtualCameras[0]; // Set initial camera
        currentCamera.gameObject.SetActive(true);
        currentTimer = idleTime;
    }

    void Update()
    {
        CheckForInputs();
    }

    private void CheckForInputs()
    {
        if (Input.anyKeyDown || Input.GetAxis("Mouse X") != 0 || Input.GetAxis("Mouse Y") != 0)
        {
            currentTimer = idleTime; // Reset timer on any input
        }
        else
        {
            currentTimer -= Time.deltaTime;
            if (currentTimer <= 0)
            {
                SwitchCamera();
                currentTimer = idleTime;
            }
        }
    }

    private void SwitchCamera()
    {
        if (virtualCameras.Count <= 1) return;

        List<CinemachineVirtualCamera> availableCameras = new List<CinemachineVirtualCamera>(virtualCameras);
        availableCameras.Remove(currentCamera); // Remove the active camera from the selection

        CinemachineVirtualCamera newCamera = availableCameras[Random.Range(0, availableCameras.Count)];
        SetActiveCamera(newCamera);
    }

    private void SetActiveCamera(CinemachineVirtualCamera newCamera)
    {
        foreach (var cam in virtualCameras)
        {
            cam.gameObject.SetActive(cam == newCamera); // Deactivate Current Camera and Set New Camera Active
        }
        currentCamera = newCamera;
    }
}
