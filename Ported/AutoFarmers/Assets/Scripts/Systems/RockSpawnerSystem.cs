using Unity.Entities;
using Unity.Collections;
using Unity.Transforms;
using Unity.Mathematics;
using UnityEngine;

partial struct RockSpawningSystem : ISystem
{
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<RockConfig>();
    }

    public void OnDestroy(ref SystemState state)
    {
    }

    public void OnUpdate(ref SystemState state)
    {
        var config = SystemAPI.GetSingleton<RockConfig>();
        var map = SystemAPI.GetSingleton<Map>();

        var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
        var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

        var rocks = CollectionHelper.CreateNativeArray<Entity>(map.mapSize.x * map.mapSize.y, Allocator.Temp);
        ecb.Instantiate(config.RockPrefab, rocks);

        var mapSize = new int2(100, 100);

        int i = 0;

        foreach (var cell in SystemAPI.Query<TransformAspect>().WithAll<Cell>())
        {
            ecb.SetComponent(rocks[i], new Translation
            {
                Value = new float3(
                cell.Position.x,
                0,
                cell.Position.z)
            });

            ecb.SetComponent(rocks[i], new NonUniformScale
            {
                Value = new float3(
                        UnityEngine.Random.Range(config.RandomSizeMin.x, config.RandomSizeMax.x),
                        UnityEngine.Random.Range(config.minHeight, config.maxHeight),
                        UnityEngine.Random.Range(config.RandomSizeMin.y, config.RandomSizeMax.y))
            });

            i++;
        }
        state.Enabled = false;
    }
}