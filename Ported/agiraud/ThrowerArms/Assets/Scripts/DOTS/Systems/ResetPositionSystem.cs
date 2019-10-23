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
[UpdateAfter(typeof(SpawnerSystem))]
public class ResetPositionSystem : JobComponentSystem
{
    EntityQuery m_GroupTinCan;
    EntityQuery m_GroupRock;

    [BurstCompile]
    struct RandomResetPositionJob : IJobForEachWithEntity<Translation, Physics, ResetPosition, Scale>
    {
        public float3 MinPosition;
        public float3 MaxPosition;
        public float3 InitialVelocity;
        public float InitialScale;
        public Random rd;
        public EntityCommandBuffer.Concurrent cb;
        public void Execute(Entity entity, int index, ref Translation position, ref Physics physics, ref ResetPosition resetPos, ref Scale scale)
        {
            if (!resetPos.needReset) return;
            position.Value = rd.NextFloat3(MinPosition, MaxPosition);
            physics.velocity = InitialVelocity;
            physics.angularVelocity = float3.zero;
            physics.flying = false;
            scale.Value = InitialScale;
            resetPos.needReset = false;
        }
    }

    protected override void OnCreate()
    {
        m_GroupTinCan = GetEntityQuery(ComponentType.ReadOnly<TinCanTag>(), 
                                                            ComponentType.ReadWrite<ResetPosition>(),
                                                            ComponentType.ReadWrite<Physics>(),
                                                            ComponentType.ReadWrite<Translation>(),
                                                            ComponentType.ReadWrite<Scale>());
        m_GroupRock = GetEntityQuery(ComponentType.ReadOnly<RockTag>(), 
                                     ComponentType.ReadWrite<ResetPosition>(),
                                     ComponentType.ReadWrite<Physics>(),
                                     ComponentType.ReadWrite<Translation>(),
                                     ComponentType.ReadWrite<Scale>());
    }

    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        var job1 = new RandomResetPositionJob()
        {
            MinPosition = RockManagerAuthoring.SpawnBoxMin,
            MaxPosition = RockManagerAuthoring.SpawnBoxMax,
            InitialVelocity = RockManagerAuthoring.MoverInitialVelocity,
            InitialScale = 1f,
            rd = new Random((uint)Environment.TickCount),
        };
        var job2 = new RandomResetPositionJob()
        {
            MinPosition = TinCanManagerAuthoring.SpawnBoxMin,
            MaxPosition = TinCanManagerAuthoring.SpawnBoxMax,
            InitialVelocity = TinCanManagerAuthoring.MoverInitialVelocity,
            InitialScale = 0f,
            rd = new Random((uint)Environment.TickCount),
        };

        var jh1 = job1.Schedule(m_GroupRock, inputDeps);
        var jh2 = job2.Schedule(m_GroupTinCan, jh1);
        return jh2;
    }
}
