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
        SpawnObstacles(state, colony);
        SpawnAnts(state, colony);
        state.Enabled = false;
    }

    void SpawnObstacles(SystemState state, Colony colony)
    {
        // We want prefabs for walls and ants. Then call instantiate and pass it to CreateEntity
        var obstacles = state.EntityManager.Instantiate(colony.obstaclePrefab, 10, Allocator.Temp);

        // alternatively add a component to the obstacle prefab that we can query on
        float n = 0;
        foreach (var transform in SystemAPI.Query<RefRW<LocalTransform>>().WithNone<Colony>())
        {
            transform.ValueRW.Position = new float3(0, 0, n*1);   
            n++;
        }
    }

    void SpawnAnts(SystemState state, Colony colony)
    {
        var ants = state.EntityManager.Instantiate(colony.antPrefab, colony.AntCount, Allocator.Temp);
        var mapSize = colony.mapSize;
        foreach (var position in SystemAPI.Query<RefRW<Position>>().WithAll<Ant>())
            position.ValueRW.position = new float2(Random.Range(-5f,5f)+mapSize*.5f,Random.Range(-5f,5f) + mapSize * .5f);
    }
}
