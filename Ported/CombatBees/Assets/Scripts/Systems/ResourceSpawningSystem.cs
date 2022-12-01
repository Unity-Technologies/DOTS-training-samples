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
        state.RequireForUpdate<Config>();

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
        var config = SystemAPI.GetSingleton<Config>();
        var halfFieldSize = config.fieldSize * .5f;

        var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
        var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

        var resources = CollectionHelper.CreateNativeArray<Entity>(config.resourceCount, Allocator.Temp);
        ecb.Instantiate(config.resourcePrefab, resources);

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

        foreach (var resource in resources)
        {
            ecb.SetComponentEnabled<ResourceCarried>(resource, false);
            ecb.SetComponentEnabled<ResourceDropped>(resource, true);
            var position = float3.zero;

            //Random, but nothing random about it really
            var random = Unity.Mathematics.Random.CreateFromIndex((uint)-resource.Index);
            var randomFloat = random.NextFloat(-1f, 1f);
            position.x = halfFieldSize.x * .4f * randomFloat;

            randomFloat = random.NextFloat(-1f, 1f);
            position.z = halfFieldSize.z * .8f * randomFloat;

            randomFloat = random.NextFloat(-1f, 1f);
            position.y = halfFieldSize.y * .3f * randomFloat;


            ecb.SetComponent(resource, new LocalTransform()
            {
                Position = position,
                Scale = 1 //If prefab scale is non-uniform, must use value of 1 to keep the original scale.
            }) ;
        }

        // This system should only run once at startup. So it disables itself after one update.
        state.Enabled = false;
    }
}