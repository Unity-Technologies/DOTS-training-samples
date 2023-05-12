using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using Unity.Entities.UniversalDelegates;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using Random = UnityEngine.Random;

public partial struct Spawner: ISystem 
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<Colony>();
        //Random.InitState(System.DateTime.UtcNow.Millisecond);
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var colony = SystemAPI.GetSingleton<Colony>();
        SpawnHome(ref state, colony);
        SpawnResource(ref state, colony);
        SpawnObstacles(ref state, colony);
        SpawnAnts(ref state, colony);
        SpawnPheromones(ref state, colony);
        state.Enabled = false;
    }

    void SpawnHome(ref SystemState state, Colony colony)
    {
        var home = state.EntityManager.Instantiate(colony.homePrefab);
        var localTransform = SystemAPI.GetComponentRW<LocalTransform>(home, false);
        localTransform.ValueRW.Position = new float3(colony.mapSize / 2f, colony.mapSize / 2f, 0f);
        state.EntityManager.AddComponent<Home>(home);
    }

    void SpawnResource(ref SystemState state, Colony colony)
    {
        var resource = state.EntityManager.Instantiate(colony.resourcePrefab);
        float mapSize = colony.mapSize;

        float resourceAngle = Random.value * 2f * Mathf.PI;
        var localTransform = SystemAPI.GetComponentRW<LocalTransform>(resource, false);
        localTransform.ValueRW.Position = new float3(mapSize * 0.5f + Mathf.Cos(resourceAngle) * mapSize * 0.475f, mapSize * 0.5f + Mathf.Sin(resourceAngle) * mapSize * 0.475f, 0);
    }

    void SpawnObstacles(ref SystemState state, Colony colony)
    {
        float mapSize = colony.mapSize;
        int ringCount = colony.ringCount;
        float obstacleRadius = colony.obstacleSize;
        float maxFillRatio = 0.8f;

        NativeList<float2> obstaclePositions = new NativeList<float2>(Allocator.Temp);

        for (int i = 1; i <= ringCount; ++i)
        {
            float ringRadius = (i / (ringCount + 1f)) * (mapSize * 0.5f);
            float circumference = ringRadius * 2f * Mathf.PI;
            int maxCount = Mathf.CeilToInt(circumference / (2f * obstacleRadius) * 2f);
            int offset = UnityEngine.Random.Range(0, maxCount);
            int holeCount = UnityEngine.Random.Range(1, 3);

            for (int j = 0; j < maxCount; ++j)
            {
                float fillRatio = (float)j / maxCount;
                // for each hole in the ring, we allow obstacles to form until maxFillRatio
                // we skip the formation of obstacles withing hole region
                // this distributes the holes uniformly
                if (((fillRatio * holeCount) % 1f) < maxFillRatio)
                {
                    float angle = (j + offset) / (float)maxCount * (2f * Mathf.PI);
                    var obstacle = state.EntityManager.Instantiate(colony.obstaclePrefab);

                    float2 obstaclePosition = new float2(mapSize * 0.5f + Mathf.Cos(angle) * ringRadius, mapSize * 0.5f + Mathf.Sin(angle) * ringRadius);

                    var localTransform = SystemAPI.GetComponentRW<LocalTransform>(obstacle, false);
                    localTransform.ValueRW.Position = new float3(obstaclePosition.x, obstaclePosition.y, 0);
                    obstaclePositions.Add(obstaclePosition);
                }
            }
        }

        int bucketResolution = colony.bucketResolution;
        //NativeArray<UnsafeList<float2>> buckets = new NativeArray<UnsafeList<float2>>(bucketResolution * bucketResolution, Allocator.Persistent);
        var buckets = SystemAPI.GetSingletonBuffer<Bucket>();
        buckets.Length = bucketResolution * bucketResolution;
        for (int i = 0; i < buckets.Length; ++i)
        {
            buckets[i] = new Bucket { obstacles = new UnsafeList<float2>(0, Allocator.Persistent) };
        }
        foreach (var position in obstaclePositions)
        {
            float radius = colony.obstacleSize;
            for (int x = Mathf.FloorToInt((position.x - radius) / mapSize * bucketResolution); x <= Mathf.FloorToInt((position.x + radius) / mapSize * bucketResolution); x++)
            {
                if (x < 0 || x >= bucketResolution)
                {
                    continue;
                }
                for (int y = Mathf.FloorToInt((position.y - radius) / mapSize * bucketResolution); y <= Mathf.FloorToInt((position.y + radius) / mapSize * bucketResolution); y++)
                {
                    if (y < 0 || y >= bucketResolution)
                    {
                        continue;
                    }
                    int index = x + y * bucketResolution;
                    var list = buckets[index].obstacles;
                    list.Add(position);
                    buckets[index] = new Bucket { obstacles = list };
                }
            }
        }

        //SystemAPI.GetSingletonRW<Colony>().ValueRW.buckets = buckets;
    }

    void SpawnAnts(ref SystemState state, Colony colony)
    {
        var ants = state.EntityManager.Instantiate(colony.antPrefab, colony.antCount, Allocator.Temp);
        var mapSize = colony.mapSize;
        foreach (var (position, direction, localTransform, speed) in SystemAPI.Query<RefRW<Position>, RefRW<Direction>, RefRW<LocalTransform>, RefRW<Speed>>().WithAll<Ant>())
        {
            position.ValueRW.position = new float2(Random.Range(-5f,5f) + mapSize * 0.5f,Random.Range(-5f,5f) + mapSize * 0.5f);
            direction.ValueRW.direction = Random.Range(0, 360);
            speed.ValueRW.speed = colony.antTargetSpeed;
            localTransform.ValueRW.Scale = colony.antScale;
        }
    }

    void SpawnPheromones(ref SystemState state, Colony colony)
    {
        var pheromones = state.EntityManager.CreateEntity();
        var pheromonesBuffer = state.EntityManager.AddBuffer<Pheromone>(pheromones);
        pheromonesBuffer.Length = (int)colony.mapSize * (int)colony.mapSize;
        for (var i = 0; i < pheromonesBuffer.Length; i++)
        {
            pheromonesBuffer[i] = new Pheromone { strength = 0f };
        }
    }
}
