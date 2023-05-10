using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public partial struct FoodSpawnerSystem : ISystem
{
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<FoodSpawnerExecution>();
        state.RequireForUpdate<GlobalSettings>();
    }

    public void OnUpdate(ref SystemState state)
    {
        state.Enabled = false;

        var settings = SystemAPI.GetSingleton<GlobalSettings>();

        var ecbSystemSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
        var ecb = ecbSystemSingleton.CreateCommandBuffer(state.WorldUnmanaged);

        foreach (var spawner in SystemAPI.Query<RefRO<FoodSpawner>, RefRW<LocalTransform>>())
        {
            for (uint i = 0; i < spawner.Item1.ValueRO.Count; i++)
            {
                var rand = Unity.Mathematics.Random.CreateFromIndex(i);
                var entity = ecb.Instantiate(spawner.Item1.ValueRO.Prefab);

                float xPos = 0;
                float yPos = 0;
                if (rand.NextBool())
                {
                    xPos = rand.NextFloat(0, settings.FoodBufferSize);
                } else
                {
                    xPos = rand.NextFloat(settings.MapSizeX - settings.FoodBufferSize, settings.MapSizeX);
                }
                if (rand.NextBool())
                {
                    yPos = rand.NextFloat(0, settings.FoodBufferSize);
                }
                else
                {
                    yPos = rand.NextFloat(settings.MapSizeY - settings.FoodBufferSize, settings.MapSizeY);
                }
                xPos = 0;
                yPos = 0;
                ecb.SetComponent(entity, new LocalTransform() { Position = new float3(xPos, yPos, 0), Scale = 1 });
                ecb.AddComponent(entity, new FoodData() {
                    Center = { x = xPos, y = yPos},
                    Radius = 1f
                });
            }
        }
    }
}

public struct FoodSpawner : IComponentData
{
    public int Count;
    public Entity Prefab;
}

public struct FoodData : IComponentData
{
    public float2 Center;
    public float Radius;
}
