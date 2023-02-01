using System.Collections.Generic;
using System.Linq;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;
using Random = UnityEngine.Random;

[BurstCompile]
partial struct HumanRoutingSystem : ISystem
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
        state.Enabled = false;
        var config = SystemAPI.GetSingleton<Config>();

        //Check for people waiting to get a route
        foreach (var human in SystemAPI.Query<RefRW<Human>, RefRO<HumanWaitForRouteTag>>())
        {
            Debug.Log("Human with tag found");
            foreach (var station in SystemAPI.Query<RefRW<Station>>())
            {
                
            }
        }
    }
}
