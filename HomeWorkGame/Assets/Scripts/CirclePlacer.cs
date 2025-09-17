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

    private GameObject[] objects;
    private float[] angles;
    private int lastCount;

    
    void Start()
    {
        if (centerObject == null)
            centerObject = this.transform;

        InitializeObjects();
    }

    void Update()
    {
        if (count != lastCount)
            InitializeObjects();
        
        
        for (var i = 0; i < Count; i++)
        {
            angles[i] += OrbitSpeed * Mathf.Deg2Rad * Time.deltaTime;

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
                if (obj != null)
                    Destroy(obj);

        objects = new GameObject[count];
        angles = new float[count];
        lastCount = count;

        var step = 2f * Mathf.PI / count;

        for (var i = 0; i < count; i++)
        {
            var angle = i * step;

            Vector3 pos = new(
                centerObject.position.x + Mathf.Cos(angle) * radius,
                centerObject.position.y,
                centerObject.position.z + Mathf.Sin(angle) * radius
            );

            objects[i] = Instantiate(prefab, pos, Quaternion.LookRotation(centerObject.position - pos), transform);
            angles[i] = angle;
        }
    }
}
