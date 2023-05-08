using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEditor;
using UnityEngine;

public partial struct SpawnerSystem: ISystem 
{
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<Colony>();
    }

    public void OnUpdate(ref SystemState state)
    {
        var colony = SystemAPI.GetSingleton<Colony>();
        
        // We want prefabs for walls and ants. Then call instantiate and pass it to CreateEntity
        var obstacles = state.EntityManager.Instantiate(colony.obstaclePrefab, 10, Allocator.Temp);

        // alternatively add a component to the obstacle prefab that we can query on
        float n = 0;
        foreach (var transform in SystemAPI.Query<RefRW<LocalTransform>>().WithNone<Colony>())
        {
            transform.ValueRW.Position = new float3(0, 0, n*1);   
            n++;
        }
        
        Debug.Log("Hello ECS");
        
        state.Enabled = false;
    }
}
