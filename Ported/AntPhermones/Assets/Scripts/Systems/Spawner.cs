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
        SpawnObstacles(state, colony);
        SpawnAnts(state, colony);
        state.Enabled = false;
    }

    void SpawnHome(SystemState state, Colony colony)
    {
        var home = state.EntityManager.Instantiate(colony.homePrefab, 1, Allocator.Temp);
    }

    void SpawnObstacles(SystemState state, Colony colony)
    {
        int mapSize = 10;
        int ringCount = 4;
        float obstacleRadius = 0.5f;
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
                    localTransform.ValueRW.Position = new float3(mapSize * 0.5f + Mathf.Cos(angle) * ringRadius, mapSize * 0.5f + Mathf.Sin(angle) * ringRadius, 0);
                }
            }
        }
    }

    void SpawnAnts(SystemState state, Colony colony)
    {
        var ants = state.EntityManager.Instantiate(colony.antPrefab, colony.antCount, Allocator.Temp);
        var mapSize = colony.mapSize;
        foreach (var position in SystemAPI.Query<RefRW<Position>>().WithAll<Ant>())
            position.ValueRW.position = new float2(Random.Range(-5f,5f),Random.Range(-5f,5f));
    }
}
