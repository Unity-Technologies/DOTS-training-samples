using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using Random = UnityEngine.Random;

public partial struct Spawner: ISystem 
{
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<Colony>();
    }

    public void OnUpdate(ref SystemState state)
    {
        var colony = SystemAPI.GetSingleton<Colony>();
        SpawnHome(state, colony);
        SpawnResource(state, colony);
        SpawnObstacles(state, colony);
        SpawnAnts(state, colony);
        state.Enabled = false;
    }

    void SpawnHome(SystemState state, Colony colony)
    {
        var home = state.EntityManager.Instantiate(colony.homePrefab);
        var localTransform = SystemAPI.GetComponentRW<LocalTransform>(home, false);
        localTransform.ValueRW.Position = new float3(colony.mapSize / 2f, colony.mapSize / 2f, 0f);
        localTransform.ValueRW.Rotation = quaternion.Euler(90f, 0f, 0f);
    }

    void SpawnResource(SystemState state, Colony colony)
    {
        var resource = state.EntityManager.Instantiate(colony.resourcePrefab);
        float mapSize = colony.mapSize;

        float resourceAngle = Random.value * 2f * Mathf.PI;
        var localTransform = SystemAPI.GetComponentRW<LocalTransform>(resource, false);
        localTransform.ValueRW.Position = new float3(Mathf.Cos(resourceAngle) * mapSize * 0.475f, Mathf.Sin(resourceAngle) * mapSize * 0.475f, 0);
    }

    void SpawnObstacles(SystemState state, Colony colony)
    {
        float mapSize = colony.mapSize;
        int ringCount = colony.ringCount;
        float obstacleRadius = colony.obstacleSize;
        float maxFillRatio = 0.8f;

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

                    var localTransform = SystemAPI.GetComponentRW<LocalTransform>(obstacle, false);
                    localTransform.ValueRW.Position = new float3(Mathf.Cos(angle) * ringRadius, Mathf.Sin(angle) * ringRadius, 0);
                }
            }
        }
    }

    void SpawnAnts(SystemState state, Colony colony)
    {
        var ants = state.EntityManager.Instantiate(colony.antPrefab, colony.antCount, Allocator.Temp);
        var mapSize = colony.mapSize;
        foreach (var (position, direction, localTransform, speed) in SystemAPI.Query<RefRW<Position>, RefRW<Direction>, RefRW<LocalTransform>, RefRW<Speed>>().WithAll<Ant>())
        {
            position.ValueRW.position = new float2(Random.Range(-5f,5f) + mapSize * 0.5f,Random.Range(-5f,5f) + mapSize * 0.5f);
            direction.ValueRW.direction = Random.Range(0, 360);
            speed.ValueRW.speed = Random.Range(0, colony.antTargetSpeed);
            localTransform.ValueRW.Scale = colony.antScale;
        }
    }
}
