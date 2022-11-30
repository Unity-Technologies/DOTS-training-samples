using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;

[BurstCompile]
partial struct ResourceSpawningSystem : ISystem
{
    private EntityQuery myQuery;

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<ResourceSpawner>();

        var builder = new EntityQueryBuilder(Allocator.Temp);
        builder.WithAll<Hive>();
        myQuery = state.GetEntityQuery(builder);
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var resourceSpawner = SystemAPI.GetSingleton<ResourceSpawner>();

        var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
        var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

        var resources = CollectionHelper.CreateNativeArray<Entity>(resourceSpawner.ResourceCount, Allocator.Temp);
        ecb.Instantiate(resourceSpawner.ResourcePrefab, resources);

        //Find the middle of all possible hives
        NativeArray<Hive> hives = myQuery.ToComponentDataArray<Hive>(Allocator.Temp);

        var totalX = 0f;
        var totalY = 0f;
        var totalZ = 0f;
        foreach (var hive in hives)
        {
            totalX += hive.boundsPosition.x;
            totalY += hive.boundsPosition.y;
            totalZ += hive.boundsPosition.z;
        }
        float3 center = new float3 { x = totalX / hives.Length, y = totalY / hives.Length, z = totalZ / hives.Length };

        foreach (var resource in resources)
        {
            ecb.SetComponentEnabled<ResourceDropped>(resource, true);
            var position = center;

            //Random, but nothing random about it really
            var random = Unity.Mathematics.Random.CreateFromIndex((uint)-resource.Index);
            var randomInt = random.NextInt(0, resourceSpawner.ResourceCount);
            position.x += -5 + (randomInt % 10);

            randomInt = random.NextInt(0, resourceSpawner.ResourceCount);
            position.z += 5 - (randomInt / 10);

            randomInt = random.NextInt(0, resourceSpawner.ResourceCount);
            position.y += 0.025f * randomInt;

            ecb.SetComponent(resource, new WorldTransform
            {
                Position = position,
                Scale = 1 //If prefab scale is non-uniform, must use value of 1 to keep the original scale.
            }) ;
        }

        // This system should only run once at startup. So it disables itself after one update.
        state.Enabled = false;
    }
}