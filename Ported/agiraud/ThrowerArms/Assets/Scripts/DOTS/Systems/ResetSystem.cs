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
public class ResetSystem : JobComponentSystem
{
    EntityQuery m_GroupTinCan;
    EntityQuery m_GroupRock;

    [BurstCompile]
    struct RandomResetPositionJob : IJobForEachWithEntity<Translation, Physics, ResetPosition, Scale, Rotation>
    {
        public float3 MinPosition;
        public float3 MaxPosition;
        public float3 InitialVelocity;
        public float InitialScale;
        public Random rd;
        public void Execute(Entity entity, int index, ref Translation position, ref Physics physics, ref ResetPosition resetPos, ref Scale scale, ref Rotation rotation)
        {
            if (!resetPos.needReset) return;
            position.Value = rd.NextFloat3(MinPosition, MaxPosition);
            physics.velocity = InitialVelocity;
            physics.angularVelocity = float3.zero;
            physics.flying = false;
            scale.Value = InitialScale;
            rotation.Value = quaternion.identity;
            resetPos.needReset = false;
        }
    }

    protected override void OnCreate()
    {
        m_GroupTinCan = GetEntityQuery(ComponentType.ReadOnly<TinCanTag>(), 
                                                            ComponentType.ReadWrite<ResetPosition>(),
                                                            ComponentType.ReadWrite<Physics>(),
                                                            ComponentType.ReadWrite<Translation>(),
                                                            ComponentType.ReadWrite<Rotation>(),
                                                            ComponentType.ReadWrite<Scale>());
        m_GroupRock = GetEntityQuery(ComponentType.ReadOnly<RockTag>(), 
                                     ComponentType.ReadWrite<ResetPosition>(),
                                     ComponentType.ReadWrite<Physics>(),
                                     ComponentType.ReadWrite<Translation>(),
                                     ComponentType.ReadWrite<Rotation>(),
                                     ComponentType.ReadWrite<Scale>());
    }

    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        var job1 = new RandomResetPositionJob()
        {
            MinPosition = SceneParameters.Instance.RockSpawnBoxMin,
            MaxPosition = SceneParameters.Instance.RockSpawnBoxMax,
            InitialVelocity = SceneParameters.Instance.RockInitialVelocity,
            InitialScale = 0.0f,
            rd = new Random((uint)Environment.TickCount),
        };
        var job2 = new RandomResetPositionJob()
        {
            MinPosition = SceneParameters.Instance.TinCanSpawnBoxMin,
            MaxPosition = SceneParameters.Instance.TinCanSpawnBoxMax,
            InitialVelocity = SceneParameters.Instance.TinCanInitialVelocity,
            InitialScale = 0.0f,
            rd = new Random((uint)Environment.TickCount),
        };

        var jh1 = job1.Schedule(m_GroupRock, inputDeps);
        var jh2 = job2.Schedule(m_GroupTinCan, jh1);
        return jh2;
    }
}
