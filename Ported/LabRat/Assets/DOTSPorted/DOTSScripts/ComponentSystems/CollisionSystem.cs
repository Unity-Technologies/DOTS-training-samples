﻿using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

[UpdateAfter(typeof(SpawningSystem))]
[UpdateInGroup(typeof(SimulationSystemGroup))]
public class CollisionSystem : SystemBase
{
    private EntityQuery m_Query;
    protected override void OnCreate()
    {
        m_Query = GetEntityQuery(typeof(Cat), typeof(Position));
        RequireForUpdate(m_Query);
    }
    
    protected override void OnUpdate()
    {
        var catPositionArray = m_Query.ToComponentDataArray<Position>(Allocator.TempJob);
        var sys = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
        var ecb = sys.CreateCommandBuffer();
        var collisionRadius = GetSingleton<GameState>().collisionRadius; 
        Entities
            .WithAll<Mouse>()
            .WithDisposeOnCompletion(catPositionArray)
            .ForEach((ref Entity entity, in Position translation) =>
        {
            for (var i=0; i<catPositionArray.Length; i++)
            {
                var dist = math.distance(translation.position, catPositionArray[i].position);
                if (dist < collisionRadius)
                {
                    ecb.DestroyEntity(entity);
                }
            }
        }).Schedule();
        sys.AddJobHandleForProducer(Dependency);
    }
}
