using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

using Unity.Collections.LowLevel.Unsafe;

public partial struct pheromoneRendereSystem : ISystem
{
    
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<PheromoneData>();
    }

    public void OnUpdate(ref SystemState state)
    {
        var pheromoneData = SystemAPI.GetSingletonBuffer<PheromoneData>();

        if (!state.EntityManager.HasComponent<PheromoneRenderer>(state.SystemHandle))
        {
            var pheromonerenderer = GameObject.Find("Map").GetComponent<PheromoneRenderer>();
            state.EntityManager.AddComponentObject(state.SystemHandle, pheromonerenderer);

        }
        else
        {
            var pheromonerenderer = state.EntityManager.GetComponentObject<PheromoneRenderer>(state.SystemHandle);
            
            NativeArray<float> data = new NativeArray<float>(pheromoneData.Length, Allocator.Temp);
            for (var i = 0; i < pheromoneData.Length; i++)
            {
                data[i] = pheromoneData[i].value;
            }
            pheromonerenderer.SetTextureData(data);
        }
        
    }
}