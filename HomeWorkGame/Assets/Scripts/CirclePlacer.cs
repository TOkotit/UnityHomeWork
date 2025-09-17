using UnityEngine;
using System.Collections.Generic;


public class CirclePlacer : MonoBehaviour
{
    [SerializeField] private GameObject prefab;
    public GameObject Prefab
    {
        get => prefab;
        set
        {
            if (value is null)
            {
                Debug.LogWarning("CirclePlacer: Prefab cannot be null.");
                return;
            }
            prefab = value;
        }
    }

    [SerializeField] private int count = 8;
    public int Count
    {
        get => count;
        set
        {
            if (value < 0)
            {
                Debug.LogError("Count can't be negative");
                return;
            }
            count = value;
        }
    }

    [SerializeField] private float radius = 5f; // Радиус окружности
    public float Radius
    {
        get => radius;
        set
        {
            if (value < 0)
            {
                Debug.LogError("Radius can't be negative");
                return;
            }
            radius = value;
        }
    }
    
    [SerializeField] private Vector3 center = Vector3.zero; // Центр окружности
    public Vector3 Center
    {
        get => center;
        set => center = value;
    }
    
    [SerializeField] private float selfRotateSpeed = 90f;
    public float SelfRotateSpeed
    {
        get => selfRotateSpeed;
        set => selfRotateSpeed = value;
    }
    
    [SerializeField] private float orbitSpeed = 30f;
    public float OrbitSpeed
    {
        get => orbitSpeed;
        set => orbitSpeed = value;
    }
    
    [SerializeField] private Transform centerObject;  
    public Transform CenterObject
    {
        get => centerObject;
        set
        {
            if (value is null)
            {
                Debug.LogWarning("CirclePlacer: CenterObject can't be null.");
                return;
            }
            centerObject = value;
        }
    }
    
    public enum Direction
    {
        Left = 1,
        Right = -1
    }
    
    [SerializeField] private Direction orbitDirection = Direction.Left;
    
    public enum PlacementMode
    {
        Evenly,   
        Sequential 
    }
    
    [SerializeField] private PlacementMode placementMode = PlacementMode.Evenly;
    [SerializeField] private float spacingBetweenPlaces = 0.9f;

    public float SpacingBetweenPlaces
    {
        get => spacingBetweenPlaces;
        set => spacingBetweenPlaces = value;
    }
    
        
    private GameObject[] objects;
    private float[] angles;
    private int lastCount;
    private PlacementMode lastPlacementMode;
    private float lastSpacing;

    void Awake()
    {
        if (centerObject == null)
            centerObject = this.transform;

        InitializeObjects();
        lastPlacementMode = placementMode;
        lastSpacing = spacingBetweenPlaces;
    }

    void Update()
    {
        if (count != lastCount ||
            Mathf.Abs(radius - Radius) > 0.001f ||
            placementMode != lastPlacementMode ||
            Mathf.Abs(spacingBetweenPlaces - lastSpacing) > 0.001f)
        {
            InitializeObjects();
            lastPlacementMode = placementMode;
            lastSpacing = spacingBetweenPlaces;
        }

        var dir = (int)orbitDirection;
        
        for (var i = 0; i < Count; i++)
        {
            angles[i] += OrbitSpeed * Mathf.Deg2Rad * dir * Time.deltaTime;

            Vector3 pos = new(
                centerObject.position.x + Mathf.Cos(angles[i]) * Radius,
                centerObject.position.y,
                centerObject.position.z + Mathf.Sin(angles[i]) * Radius
            );

            objects[i].transform.position = pos;
            objects[i].transform.Rotate(Vector3.up, selfRotateSpeed * Time.deltaTime, Space.Self);
        }
    }
    
    private void InitializeObjects()
    {
        // Удаляем старые объекты
        if (objects != null)
            foreach (var obj in objects)
                if (obj)
                    Destroy(obj);

        objects = new GameObject[Count];
        angles = new float[Count];
        lastCount = Count;

        var step = 2f * Mathf.PI / Count;
        
        for (var i = 0; i < Count; i++)
        {
            var angle = i * step;
            if (placementMode == PlacementMode.Evenly)
                angle = i * step;
            else if (placementMode == PlacementMode.Sequential)
                angle = i * SpacingBetweenPlaces;
            Vector3 pos = new(
                centerObject.position.x + Mathf.Cos(angle) * Radius,
                centerObject.position.y,
                centerObject.position.z + Mathf.Sin(angle) * Radius
            );

            objects[i] = Instantiate(Prefab, pos, Quaternion.LookRotation(centerObject.position - pos), transform);
            angles[i] = angle;
        }
    }
}
