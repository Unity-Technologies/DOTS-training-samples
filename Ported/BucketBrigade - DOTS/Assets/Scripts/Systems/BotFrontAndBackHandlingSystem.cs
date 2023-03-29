using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;

[UpdateAfter(typeof(BucketMovingSystem))]
[BurstCompile]
public partial struct BotFrontAndBackHandlingSystem : ISystem
{

    public void OnCreate(ref SystemState state)
    { 
        state.RequireForUpdate<Config>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        //Find all front bots and make them set the team's emptying bucket to its own transform 

        foreach (var VARIABLE in SystemAPI.Query<LocalTransform>().WithAll<Team,FreeTag>())
        {
            
        }
    
        
    }
}