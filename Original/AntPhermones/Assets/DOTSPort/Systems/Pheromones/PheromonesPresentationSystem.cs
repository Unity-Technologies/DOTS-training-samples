using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public partial struct PheromonesPresentationInitSystem : ISystem
{
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<GlobalSettings>();
        state.RequireForUpdate<PheromonesPresentationTexture>();
    }

    public void OnUpdate(ref SystemState state)
    {
        state.Enabled = false;

        var settingsSingleton = SystemAPI.GetSingleton<GlobalSettings>();
        var presentationEntity = SystemAPI.GetSingletonEntity<PheromonesPresentationTexture>();
        SystemAPI.SetComponent(presentationEntity, new LocalToWorld() { Value = float4x4.TRS(
            translation: new float3(0f, 0f, 0f),
            rotation: quaternion.identity,
            scale: new float3(settingsSingleton.MapSizeX, settingsSingleton.MapSizeY, 1)
        )});
        
        // TODO: Why code bellow doesn't work?
 //       var localToWorld = SystemAPI.GetComponent<LocalToWorld>(presentationEntity);
 //       localToWorld.Value = float4x4.TRS(
 //           translation: new float3(0f, 0f, 0f),
 //           rotation: quaternion.identity,
 //           scale: new float3(settingsSingleton.MapSizeX, settingsSingleton.MapSizeY, 1)
 //       );
       // TODO: How to get particular mesh on particular Entity?
       
       // TODO: Regenerate mesh quad
    }
}

public partial struct PheromonesPresentationSystem : ISystem
{
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<GlobalSettings>();
        state.RequireForUpdate<PheromonesPresentationTexture>();
        state.RequireForUpdate<PheromoneBufferElement>();
    }

    public void OnUpdate(ref SystemState state)
    {
        var pheromoneBufferElement = SystemAPI.GetSingletonBuffer<PheromoneBufferElement>();
        
        // TODO: how to get texture from Entity?
    }
}



