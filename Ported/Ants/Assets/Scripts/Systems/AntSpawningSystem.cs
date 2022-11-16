using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using Random = Unity.Mathematics.Random;


[BurstCompile]
public partial struct AntSpawningSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var rand = new Random(123);

        var config = SystemAPI.GetSingleton<Config>();

        var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
        var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

        var ants = CollectionHelper.CreateNativeArray<Entity>(config.Amount, Allocator.Temp);
        ecb.Instantiate(config.AntPrefab, ants);
        var xPos = config.MapSize * 0.5f;
        var yPos = config.MapSize * 0.5f;

        foreach (var ant in ants)
        {
            var antComponent = new Ant()
            {
                Position = new float2(xPos, yPos),
                Speed = 5,
                Angle = rand.NextFloat(0f, 360f),
                HasFood = false
            };
            ecb.AddComponent(ant, antComponent);

            var local = new LocalToWorldTransform();
            
            local.Value.Position = new float3(xPos, yPos, 0f);
            local.Value.Scale = 1f;
            ecb.SetComponent(ant, local);

            var postTransform = new PostTransformMatrix()
            {
                Value = float4x4.Scale(1f, 0.5f, 0.5f)
            };
            ecb.AddComponent(ant,postTransform);
        }

        state.Enabled = false;
    }
}