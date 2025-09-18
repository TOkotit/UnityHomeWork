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

    [SerializeField] private float radius = 5f; 
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
    
    [SerializeField] private Vector3 center = Vector3.zero;
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
    
    private enum Direction
    {
        Left = 1,
        Right = -1
    }
    
    [SerializeField] private Direction orbitDirection = Direction.Left;
    
    private enum PlacementMode
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
    
        
    private GameObject[] _objects;
    private float[] _angles;
    private int _lastCount;
    private PlacementMode _lastPlacementMode;
    private float _lastSpacing;

    private void Awake()
    {
        centerObject ??= this.transform;

        InitializeObjects();
        _lastPlacementMode = placementMode;
        _lastSpacing = spacingBetweenPlaces;
    }

    private void Update()
    {
        if (count != _lastCount ||
            Mathf.Abs(radius - Radius) > 0.001f ||
            placementMode != _lastPlacementMode ||
            Mathf.Abs(spacingBetweenPlaces - _lastSpacing) > 0.001f)
        {
            InitializeObjects();
            _lastPlacementMode = placementMode;
            _lastSpacing = spacingBetweenPlaces;
        }

        var dir = (int)orbitDirection;
        
        for (var i = 0; i < Count; i++)
        {
            _angles[i] += OrbitSpeed * Mathf.Deg2Rad * dir * Time.deltaTime;

            Vector3 pos = new(
                centerObject.position.x + Mathf.Cos(_angles[i]) * Radius,
                centerObject.position.y,
                centerObject.position.z + Mathf.Sin(_angles[i]) * Radius
            );

            _objects[i].transform.position = pos;
            _objects[i].transform.Rotate(Vector3.up, selfRotateSpeed * Time.deltaTime, Space.Self);
        }
    }
    
    private void InitializeObjects()
    {
        if (_objects != null)
            foreach (var obj in _objects)
                if (obj)
                    Destroy(obj);

        _objects = new GameObject[Count];
        _angles = new float[Count];
        _lastCount = Count;

        var step = 2f * Mathf.PI / Count;
        
        for (var i = 0; i < Count; i++)
        {
            var angle = placementMode switch
            {
                PlacementMode.Evenly => i * step,
                PlacementMode.Sequential => i * SpacingBetweenPlaces,
                _ => i * step
            };
            Vector3 pos = new(
                centerObject.position.x + Mathf.Cos(angle) * Radius,
                centerObject.position.y,
                centerObject.position.z + Mathf.Sin(angle) * Radius
            );

            _objects[i] = Instantiate(Prefab, pos, Quaternion.LookRotation(centerObject.position - pos), transform);
            _angles[i] = angle;
        }
    }
}
