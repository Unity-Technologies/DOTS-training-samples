using System.ComponentModel.Design.Serialization;
using Unity.Burst;
using Unity.Entities;
using Unity.Collections;
using Unity.Jobs;
// using UnityEngine;  
using Unity.Mathematics;
using Unity.Transforms;


[BurstCompile]
partial struct ColonySpawnSystem : ISystem
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
        var config = SystemAPI.GetSingleton<Config>();
        float3 middlePosition = new float3(config.MapSize / 2f, config.MapSize / 2f, 0f); 
        
        var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
        var ECB = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

        // spawn entity 
        var colony = ECB.Instantiate(config.ColonyPrefab);
        LocalToWorldTransform colonyTransform = new LocalToWorldTransform();
        colonyTransform.Value.Position = middlePosition;
        colonyTransform.Value.Scale = config.WallRadius; // need to scale or it equals zero 
        ECB.SetComponent(colony, colonyTransform);
        ECB.AddComponent<Colony>(colony); // add the colony tag 

        // run once
        state.Enabled = false; 
    }
}