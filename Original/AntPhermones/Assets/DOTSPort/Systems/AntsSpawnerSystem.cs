using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public partial struct AntsSpawnerSystem : ISystem
{
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<AntSpawnerExecution>();
    }

    public void OnUpdate(ref SystemState state)
    {
        state.Enabled = false;
        
        var ecbSystemSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
        var ecb = ecbSystemSingleton.CreateCommandBuffer(state.WorldUnmanaged);
        
        foreach (var spawner in SystemAPI.Query<RefRO<AntSpawner>, RefRW<LocalTransform>>())
        {
            for (int i = 0; i < spawner.Item1.ValueRO.Count; i++)
            {
                var entity = ecb.Instantiate(spawner.Item1.ValueRO.Prefab);
                ecb.SetComponent(entity, new LocalTransform() { Position = spawner.Item2.ValueRO.Position + new float3(1,i, 5), Scale = 1});
            }
        }
    }
}

public struct AntSpawner : IComponentData
{
    public int Count;
    public Entity Prefab;
}
