using Components;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using Random = Unity.Mathematics.Random;

[BurstCompile]
partial struct BeeSpawningSystem : ISystem
{
    Random random;
    
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        random = Random.CreateFromIndex(1234);
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var spawner = SystemAPI.GetSingleton<InitialSpawn>();

        var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
        var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

        var blueBees = CollectionHelper.CreateNativeArray<Entity>(spawner.beeCount/2, Allocator.Temp);
        var yellowBees = CollectionHelper.CreateNativeArray<Entity>(spawner.beeCount/2, Allocator.Temp);
        var food = CollectionHelper.CreateNativeArray<Entity>(spawner.foodCount, Allocator.Temp);

        var baseComponent = SystemAPI.GetSingleton<Base>();
        
        ecb.Instantiate(spawner.blueBeePrefab, blueBees);
        
        foreach (var bee in blueBees)
        {
            var blueBaseCache = baseComponent.blueBase;
            var randomSpawn = random.NextFloat3(blueBaseCache.GetBaseLowerLeftCorner(), blueBaseCache.GetBaseRightCorner());

            ecb.SetComponent(bee, new Translation
            {
                Value = randomSpawn
            });
        }

        ecb.Instantiate(spawner.yellowBeePrefab, yellowBees);
        
        foreach (var bee in yellowBees)
        {
            var yellowBaseCache = baseComponent.yellowBase;

            var randomSpawn = random.NextFloat3(yellowBaseCache.GetBaseLowerLeftCorner(), yellowBaseCache.GetBaseRightCorner());

            ecb.SetComponent(bee, new Translation
            {
                Value = randomSpawn
            });
        }
        
        ecb.Instantiate(spawner.foodPrefab, food);
        
        foreach (var foodNode in food)
        {
            ecb.SetComponent(foodNode, new Translation
            {
                Value = new float3(0, 0 ,0)
            });
        }

        state.Enabled = false;
    }
}
