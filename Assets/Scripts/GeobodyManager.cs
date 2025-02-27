using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class GeobodyManager : MonoBehaviour
{
    public static GeobodyManager Instance;

    [Header("Spawn Settings")]
    [SerializeField] private float looseCount = 12f;
    [SerializeField] private float maxCount = 25f;
    [SerializeField] private float edgeToSpawnDistance = 2f;

    [Header("Asset Reference")]
    [SerializeField] private List<GameObject> geobodies = new List<GameObject>();
    [SerializeField] private List<Material> stippledMaterials = new List<Material>();

    [Header("Geobody Tracking")]
    public List<Geobody> looseGeobodies = new List<Geobody>();
    public List<Geobody> snappedGeobodies = new List<Geobody>();

    private List<GameObject> geobodyPool = new List<GameObject>();
    private List<Material> materialPool = new List<Material>();

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

        geobodyPool.AddRange(geobodies);
        materialPool.AddRange(stippledMaterials);
        backgroundLayer = LayerMask.GetMask("Background");
    }

    void Update()
    {
        if (looseGeobodies.Count < looseCount && looseGeobodies.Count+snappedGeobodies.Count < maxCount)
        {
            InstantiateGeobody(GetScreenEdgePosition());
        }
    }

    private Vector3 GetScreenEdgePosition()
    {
        Vector3 screenPosition = Vector3.zero;

        float offset = edgeToSpawnDistance * Screen.height/(2*Camera.main.orthographicSize);
        float width = Random.Range(-offset, Screen.width + offset);
        float height = Random.Range(-offset, Screen.height + offset);
        int edge = Random.Range(0, 4);
        switch (edge)
        {
            case 0: // Top
                screenPosition = new Vector3(width, Screen.height + offset, Camera.main.nearClipPlane);
                break;
            case 1: // Bottom
                screenPosition = new Vector3(width, - offset, Camera.main.nearClipPlane);
                break;
            case 2: // Left
                screenPosition = new Vector3(-offset, height, Camera.main.nearClipPlane);
                break;
            case 3: // Right
                screenPosition = new Vector3(Screen.width + offset, height, Camera.main.nearClipPlane);
                break;
        }

        RaycastHit spawnHit;
        Vector3 rayDirection = Camera.main.ScreenToWorldPoint(new Vector3(0f, 0f, Camera.main.farClipPlane - Camera.main.nearClipPlane));
        Physics.Raycast(Camera.main.ScreenToWorldPoint(screenPosition), rayDirection, out spawnHit, Mathf.Infinity, backgroundLayer);
        return spawnHit.point;
    }

    public Material PickMaterial()
    {
        int selectedElement = Random.Range(0, materialPool.Count);
        var selectedMaterial = materialPool[selectedElement];
        materialPool.Remove(selectedMaterial);

        if (materialPool.Count == 0) materialPool.AddRange(stippledMaterials);

        return selectedMaterial;
    }

    private void InstantiateGeobody(Vector3 position)
    {
        int selectedElement = Random.Range(0, geobodyPool.Count);
        Instantiate(geobodyPool[selectedElement], position, Quaternion.identity);
        geobodyPool.RemoveAt(selectedElement);
        if (geobodyPool.Count == 0) geobodyPool.AddRange(geobodies);
    }
}
