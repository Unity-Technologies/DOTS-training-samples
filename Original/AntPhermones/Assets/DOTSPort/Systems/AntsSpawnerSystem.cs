using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using Unity.Rendering;

public partial struct AntsSpawnerSystem : ISystem
{
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<AntSpawnerExecution>();
        state.RequireForUpdate<GlobalSettings>();
        state.RequireForUpdate<FoodData>();
    }

    public void OnUpdate(ref SystemState state)
    {
        state.Enabled = false;
        

        var ecbSystemSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
        var ecb = ecbSystemSingleton.CreateCommandBuffer(state.WorldUnmanaged);

        var globalSettings = SystemAPI.GetSingleton<GlobalSettings>();
        var food = SystemAPI.GetSingleton<FoodData>();

        float fGridSizeScalar = (float)math.min(globalSettings.MapSizeX, globalSettings.MapSizeY) / 128.0f;

        foreach (var spawner in SystemAPI.Query<RefRO<AntSpawner>, RefRW<LocalTransform>>())
        {
            // Set spawner to the center of the map.
            // If we have more than one spawner we should find another way to set position
            spawner.Item2.ValueRW.Position = new float3(globalSettings.MapSizeX / 2f, globalSettings.MapSizeY / 2f, 4); 
            
            var antSpawnRange = spawner.Item1.ValueRO.AntSpawnRange;
            for (uint i = 0; i < spawner.Item1.ValueRO.Count; i++)
            {
                var rand = Unity.Mathematics.Random.CreateFromIndex(i);
                var entity = ecb.Instantiate(spawner.Item1.ValueRO.Prefab);
                ecb.SetComponent(entity, new LocalTransform() { Position = new float3(
                    spawner.Item2.ValueRO.Position.x + rand.NextFloat(-antSpawnRange,antSpawnRange),
                    spawner.Item2.ValueRO.Position.y + rand.NextFloat(-antSpawnRange,antSpawnRange),
                    0),
                    Scale = fGridSizeScalar});
                ecb.AddComponent(entity, new AntData() {
                    SpawnerCenter = { 
                        x = spawner.Item2.ValueRO.Position.x, 
                        y = spawner.Item2.ValueRO.Position.y 
                    },
                    FacingAngle = rand.NextFloat() * Mathf.PI * 2f,
                    Speed = 0,
                    HoldingResource = false,
                    Rand = rand,
                    TargetPosition = food.Center
                });
                
                ecb.AddComponent(entity, new URPMaterialPropertyBaseColor() { Value = globalSettings.RegularColor });
            }
        }
    }
}

public struct AntSpawner : IComponentData
{
    public int Count;
    public float AntSpawnRange;
    public Entity Prefab;
}

public struct AntData : IComponentData
{
    public float2 SpawnerCenter;
    public float2 TargetPosition;
    public float FacingAngle;
    public float Speed;
    public bool HoldingResource;
    public Unity.Mathematics.Random Rand;
}
