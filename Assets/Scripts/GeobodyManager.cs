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
    [SerializeField] private float spawnInterval = 2f;

    [Header("Asset Reference")]
    [SerializeField] private List<GameObject> geobodies = new List<GameObject>();
    [SerializeField] private List<Material> stippledMaterials = new List<Material>();

    [Header("Geobody Tracking")]
    public List<Geobody> looseGeobodies = new List<Geobody>();
    public List<Geobody> snappedGeobodies = new List<Geobody>();

    private float timer;

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
        if (looseGeobodies.Count < looseCount && looseGeobodies.Count + snappedGeobodies.Count < maxCount && timer == spawnInterval)
        {
            InstantiateGeobody(GetScreenEdgePosition());
        }

        timer -= Time.deltaTime;
        if (timer < 0f) timer = spawnInterval;
    }

    private Vector3 GetScreenEdgePosition()
    {
        Vector3 screenPosition = Vector3.zero;

        float offset = edgeToSpawnDistance * Screen.height / (2 * Camera.main.orthographicSize);
        float width = Random.Range(0f, Screen.width);
        float height = Random.Range(0f, Screen.height);
        int edge = GetWeightedEdge();
        switch (edge)
        {
            case 0: // Top
                screenPosition = new Vector3(width, Screen.height + offset, Camera.main.nearClipPlane);
                break;
            case 1: // Bottom
                screenPosition = new Vector3(width, -offset, Camera.main.nearClipPlane);
                break;
            case 2: // Left
                screenPosition = new Vector3(-offset, height, Camera.main.nearClipPlane);
                break;
            case 3: // Right
                screenPosition = new Vector3(Screen.width + offset, height, Camera.main.nearClipPlane);
                break;
        }

        RaycastHit spawnHit;
        Vector3 rayDirection = Camera.main.ScreenToWorldPoint(new Vector3(screenPosition.x, screenPosition.y, Camera.main.farClipPlane)) - Camera.main.ScreenToWorldPoint(screenPosition);
        Physics.Raycast(Camera.main.ScreenToWorldPoint(screenPosition), rayDirection, out spawnHit, Mathf.Infinity, backgroundLayer);
        return (spawnHit.point - rayDirection.normalized * 1f);
    }

    int GetWeightedEdge()
    {
        float totalWeight = 0f;
        float sideWeight = Screen.height / Screen.width;
        float[] weights = { 1f, 1f, sideWeight, sideWeight };

        for (int i = 0; i < weights.Length; i++)
        {
            totalWeight += weights[i];
        }

        float randomValue = Random.Range(0f, totalWeight);
        float weightSum = 0f;
        int selectedEdge = 0;

        for (int i = 0; i < weights.Length; i++)
        {
            weightSum += weights[i];
            if (randomValue <= weightSum)
            {
                selectedEdge = i;
                break;
            }
        }

        return selectedEdge;
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
        Instantiate(geobodyPool[selectedElement], position, Quaternion.LookRotation(transform.up, transform.forward));
        geobodyPool.RemoveAt(selectedElement);
        if (geobodyPool.Count == 0) geobodyPool.AddRange(geobodies);
    }
}
