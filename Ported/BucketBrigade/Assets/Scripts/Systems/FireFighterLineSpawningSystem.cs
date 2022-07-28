using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Collections;
using UnityEngine;


[BurstCompile]
partial struct FireFighterLineSpawnSystem : ISystem
{
  
    
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        // This system should not run before the Config singleton has been loaded.
        state.RequireForUpdate<FireFighterLineConfig>();
        state.RequireForUpdate<FireFighterConfig>();
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
    }

    // See note above regarding the [BurstCompile] attribute.
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var config = SystemAPI.GetSingleton<FireFighterLineConfig>();
        var ffConfig = SystemAPI.GetSingleton<FireFighterConfig>();
        var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
        var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

        var ffLines = CollectionHelper.CreateNativeArray<Entity>(ffConfig.LinesCount, Allocator.Temp);
        ecb.Instantiate(config.Prefab, ffLines);
        int index = 0;
        // update ids
        foreach (var ffline in ffLines)
        {
            
            ecb.SetComponent(ffline, new FireFighterLine { LineId = index++ });
        }
        
        state.Enabled = false;
        
    }
}