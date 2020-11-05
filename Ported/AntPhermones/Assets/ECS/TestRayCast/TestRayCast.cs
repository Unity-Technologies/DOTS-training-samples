using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class TestRayCast : MonoBehaviour
{
    public static EntityManager EntityManager;

    [SerializeField] float obstaclesRadius = 1f;
    [SerializeField] float targetRadius = 1f;
    [SerializeField] Transform targetTransform;
    [SerializeField] Transform obstacleRoot;

    //Replacement of Vector3.up
    static readonly float3 v3Up = new float3(0, 1, 0);

    new Camera camera;
    float3 origin;
    DynamicBuffer<ObstaclePosition> obstaclePositions;

    private void Awake()
    {
        camera = Camera.main;
    }

    private void Start()
    {
        var entity = EntityManager.CreateEntity();
        obstaclePositions = EntityManager.AddBuffer<ObstaclePosition>(entity);

        foreach (Transform obstacle in obstacleRoot)
        {
            obstaclePositions.Add(new ObstaclePosition { Value = obstacle.position });
        }

        Debug.Log(obstaclePositions.Length + " obstacles found");
    }

    private void Update()
    {
        var ray = camera.ScreenPointToRay(Input.mousePosition);


        var plane = new Plane(v3Up, 0);

        float t;
        if (plane.Raycast(ray, out t))
        {
            var hit = ray.GetPoint(t);
            origin = hit;
        }
    }

    private void OnDrawGizmos()
    {
        if (!obstaclePositions.IsCreated) return;

        var from = float2.zero;
        from.x = origin.x;
        from.y = origin.z;

        var to = float2.zero;
        to.x = targetTransform.position.x;
        to.y = targetTransform.position.z;

        if (AntMovementSystem.RayCast(from, to, targetRadius, obstaclesRadius, obstaclePositions))
        {
            Gizmos.color = Color.red;
        }
        else
        {
            Gizmos.color = Color.green;
        }

        Gizmos.DrawLine(origin, targetTransform.position);
    }
}
