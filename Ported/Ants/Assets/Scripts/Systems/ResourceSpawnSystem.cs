using System;
using System.ComponentModel.Design.Serialization;
using Unity.Burst;
using Unity.Entities;
using Unity.Collections;
using Unity.Jobs;
// using UnityEngine;  
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using Random = Unity.Mathematics.Random;


[BurstCompile]
partial struct ResourceSpawnSystem : ISystem
{
    private uint time;

    public void OnCreate(ref SystemState state)
    {
        time = (uint)DateTime.Now.Millisecond;

    }
 
    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var config = SystemAPI.GetSingleton<Config>();
        
        // get resource position 
        Random rand = Random.CreateFromIndex(time); // todo: figure out how to make this a random number 
        float resourceAngle = rand.NextFloat() * 2f * math.PI;
        float3 resourcePosition = new float3(new float2(config.MapSize * 0.5f, config.MapSize * 0.5f) + new float2(math.cos(resourceAngle) * config.MapSize * .475f,
            math.sin(resourceAngle) * config.MapSize * .475f), 0f);
        
        var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
        var ECB = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

        // spawn entity 
        var resource = ECB.Instantiate(config.ResourcePrefab);
        LocalToWorldTransform resourceTransform = new LocalToWorldTransform();
        resourceTransform.Value.Position = resourcePosition;
        resourceTransform.Value.Scale = config.WallRadius; // need to scale or it equals zero 
        config.ResourcePoint = resourceTransform.Value.Position;
        ECB.SetComponent(resource, resourceTransform);
        ECB.AddComponent<Resource>(resource); // add the colony tag 

        // run once
        state.Enabled = false; 
    }
}