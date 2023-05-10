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
        // TODO : make antSpawnRange configurable
        const float antSpawnRange = 1f;
        state.Enabled = false;
        
        var ecbSystemSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
        var ecb = ecbSystemSingleton.CreateCommandBuffer(state.WorldUnmanaged);

        foreach (var spawner in SystemAPI.Query<RefRO<AntSpawner>, RefRW<LocalTransform>>())
        {
            for (uint i = 0; i < spawner.Item1.ValueRO.Count; i++)
            {
                var rand = Unity.Mathematics.Random.CreateFromIndex(i);
                var entity = ecb.Instantiate(spawner.Item1.ValueRO.Prefab);
                ecb.SetComponent(entity, new LocalTransform() { Position = new float3(0,0,0), Scale = 1});
                ecb.AddComponent(entity, new AntData() {
                    SpawnerCenter = { 
                        x = spawner.Item2.ValueRO.Position.x, 
                        y = spawner.Item2.ValueRO.Position.y 
                    },
                    Position = { 
                        x = rand.NextFloat(-antSpawnRange,antSpawnRange), 
                        y = rand.NextFloat(-antSpawnRange,antSpawnRange) 
                    },
                    FacingAngle = rand.NextFloat() * Mathf.PI * 2f,
                    Speed = 0,
                    HoldingResource = false,
                    Brightness = rand.NextFloat(.75f, 1.25f),
                    Rand = rand
                });
            }
        }
    }
}

public struct AntSpawner : IComponentData
{
    public int Count;
    public Entity Prefab;
}

public struct AntData : IComponentData
{
    public Vector2 SpawnerCenter;
    public Vector2 Position;
    public float FacingAngle;
    public float Speed;
    public bool HoldingResource;
    public float Brightness;
    public Unity.Mathematics.Random Rand;
}
