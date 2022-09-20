using System;
using Unity.Burst;
using Unity.Collections;
using Unity.Burst.Intrinsics;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEditor;
using UnityEngine;

[BurstCompile]
public struct MovementJob : IJobChunk
{
    public ComponentTypeHandle<LocalToWorldTransform> TransformHandle;
    public ComponentTypeHandle<Velocity> VelocityHandle;

    [ReadOnly] public float TimeStep;
    [ReadOnly] public float Gravity;

    public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
    {
        var transforms = chunk.GetNativeArray(TransformHandle);
        var velocities = chunk.GetNativeArray(VelocityHandle);

        var gravityDt = TimeStep * Gravity; 
        for (int i = 0; i < chunk.Count; ++i)
        {
            var tmComp = transforms[i];
            var velComp = velocities[i];

            var velNew = velComp.Value;
            velNew.y += gravityDt; 
            var pos = tmComp.Value.Position;
            pos += TimeStep * velNew;
            // bounce off lower bottom
            pos.y *= Convert.ToSingle(pos.y > 0);

            tmComp.Value.Position = pos;
            velComp.Value = velNew;

            transforms[i] = tmComp;
            velocities[i] = velComp;
        }
    }
}

[BurstCompile]
partial struct MovementSystem : ISystem
{
    private EntityQuery Query;
    public ComponentTypeHandle<LocalToWorldTransform> TransformHandle;
    public ComponentTypeHandle<Velocity> VelocityHandle;

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        Query = state.GetEntityQuery(typeof(LocalToWorldTransform), typeof(Velocity));
        TransformHandle = state.GetComponentTypeHandle<LocalToWorldTransform>();
        VelocityHandle = state.GetComponentTypeHandle<Velocity>();
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        TransformHandle.Update(ref state);
        VelocityHandle.Update(ref state);
        var job = new MovementJob()
        {
            TransformHandle = TransformHandle,
            VelocityHandle = VelocityHandle,
            TimeStep = state.Time.DeltaTime,
            Gravity = -9.81f
        };

        state.Dependency = job.ScheduleParallel(Query, state.Dependency);
    }
}