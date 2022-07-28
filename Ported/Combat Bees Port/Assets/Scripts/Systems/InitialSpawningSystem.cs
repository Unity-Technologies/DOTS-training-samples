using Components;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using Random = Unity.Mathematics.Random;

[BurstCompile]
partial struct InitialSpawningSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<InitialSpawn>();
        state.RequireForUpdate<Base>();
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var random = Random.CreateFromIndex(state.GlobalSystemVersion);

        var spawner = SystemAPI.GetSingleton<InitialSpawn>();

        var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
        var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

        var blueBees = CollectionHelper.CreateNativeArray<Entity>(spawner.beeCount/2, Allocator.Temp);
        var yellowBees = CollectionHelper.CreateNativeArray<Entity>(spawner.beeCount/2, Allocator.Temp);
        var food = CollectionHelper.CreateNativeArray<Entity>(spawner.foodCount, Allocator.Temp);

        var baseComponent = SystemAPI.GetSingleton<Base>();
        
        ecb.Instantiate(spawner.blueBeePrefab, blueBees);
        ecb.Instantiate(spawner.yellowBeePrefab, yellowBees);
        ecb.Instantiate(spawner.foodPrefab, food);

        var blueBaseCache = baseComponent.blueBase;
        
        foreach (var bee in blueBees)
        {
            var randomSpawn = random.NextFloat3(blueBaseCache.GetBaseLowerLeftCorner(), blueBaseCache.GetBaseUpperRightCorner());
            
            ecb.SetComponent(bee, new Translation
            {
                Value = randomSpawn
            });
        }
        
        var yellowBaseCache = baseComponent.yellowBase;

        foreach (var bee in yellowBees)
        {
            var randomSpawn = random.NextFloat3(yellowBaseCache.GetBaseLowerLeftCorner(), yellowBaseCache.GetBaseUpperRightCorner());
            ecb.SetComponent(bee, new Translation
            {
                Value = randomSpawn
            });
        }
        
        foreach (var foodNode in food)
        {
            var randomSpawn = random.NextFloat3(new float3(-15, 0, -10), new float3(15, 0, 10));

            ecb.SetComponent(foodNode, new Translation
            {
                Value = randomSpawn
            });
        }

        state.Enabled = false;
    }
}
