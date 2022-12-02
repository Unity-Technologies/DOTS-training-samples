using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;

[BurstCompile]
partial struct RailywaySpawner : ISystem
{
    EntityQuery entityQuery;
    bool initialised;

    NativeArray<RailwayPoint> railwayPoints;
    NativeArray<LocalToWorld> transforms;

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        initialised = false;
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
        railwayPoints.Dispose();
        transforms.Dispose();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        if (!initialised)
        {
            entityQuery = SystemAPI.QueryBuilder().WithAll<RailwayPoint, LocalToWorld>().Build();

            railwayPoints = entityQuery.ToComponentDataArray<RailwayPoint>(Allocator.Persistent);
            transforms = entityQuery.ToComponentDataArray<LocalToWorld>(Allocator.Persistent);

            initialised = true;
        }

        for (int i = 0; i < railwayPoints.Length; i++)
        {
            float3 startPoint;
            float3 endPoint;

            var railwayPoint = railwayPoints[i];

            startPoint = railwayPoint.PreviousPoint;
            endPoint = railwayPoint.NextPoint;

            Debug.DrawLine(startPoint, endPoint);
        }
    }
}