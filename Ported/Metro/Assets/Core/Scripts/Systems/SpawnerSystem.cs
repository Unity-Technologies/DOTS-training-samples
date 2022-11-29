using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

[BurstCompile]
partial struct SpawnerSystem : ISystem
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
            var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
            var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);
            var config = SystemAPI.GetSingleton<Config>();
            var persons = CollectionHelper.CreateNativeArray<Entity>(config.PersonCount, Allocator.Temp);
            Debug.Log("persons: " + persons.Length);
            ecb.Instantiate(config.PersonPrefab, persons);
        state.Enabled = false;
    }
}