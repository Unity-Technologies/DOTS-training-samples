using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.Rendering;
using Random = Unity.Mathematics.Random;

[UpdateInGroup(typeof(ThrowerArmsGroupSystem))]
[UpdateAfter(typeof(ResetPositionSystem))]
public class AfterResetPositionCommandBufferSystem : EntityCommandBufferSystem { }

[UpdateInGroup(typeof(ThrowerArmsGroupSystem))]
[UpdateAfter(typeof(SpawnerSystem))]
public class ResetPositionSystem : JobComponentSystem
{
    EntityQuery m_GroupTinCan;
    EntityQuery m_GroupRock;
    AfterResetPositionCommandBufferSystem m_EntityCommandBufferSystem;

    //[BurstCompile]
    struct RandomResetPositionJob : IJobForEachWithEntity<Translation>
    {
        public float3 MinPosition;
        public float3 MaxPosition;
        public Random rd;
        public EntityCommandBuffer.Concurrent cb;
        public void Execute(Entity entity, int index, ref Translation position)
        {
            position.Value = rd.NextFloat3(MinPosition, MaxPosition);
            cb.RemoveComponent<ResetPosition>(index, entity);
        }
    }

    protected override void OnCreate()
    {
        m_GroupTinCan = GetEntityQuery(ComponentType.ReadOnly<TinCanTag>(), 
                                                            ComponentType.ReadOnly<ResetPosition>(),
                                                            ComponentType.ReadWrite<Translation>());
        m_GroupRock = GetEntityQuery(ComponentType.ReadOnly<RockTag>(), 
                                     ComponentType.ReadOnly<ResetPosition>(), 
                                     ComponentType.ReadWrite<Translation>());

        m_EntityCommandBufferSystem = World.GetOrCreateSystem<AfterResetPositionCommandBufferSystem>();
    }

    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        var job1 = new RandomResetPositionJob()
        {
            MinPosition = new float3(-20, 0f, 0f),
            MaxPosition = new float3(-150, 0f, 0f),
            rd = new Random((uint)Environment.TickCount),
            cb = m_EntityCommandBufferSystem.CreateCommandBuffer().ToConcurrent()
        };
        var job2 = new RandomResetPositionJob()
        {
            MinPosition = new float3(-10, 3f, 15f),
            MaxPosition = new float3(-100, 8f, 15f),
            rd = new Random((uint)Environment.TickCount),
            cb = m_EntityCommandBufferSystem.CreateCommandBuffer().ToConcurrent()
        };

        var jh1 = job1.Schedule(m_GroupRock, inputDeps);
        var jh2 = job2.Schedule(m_GroupTinCan, jh1);
        m_EntityCommandBufferSystem.AddJobHandleForProducer(jh1);
        m_EntityCommandBufferSystem.AddJobHandleForProducer(jh2);
        return jh2;
    }
}
