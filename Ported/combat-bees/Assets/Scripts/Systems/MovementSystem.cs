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
    [ReadOnly] public ComponentTypeHandle<Velocity> VelocityHandle;

    [ReadOnly] public float TimeStep;
    [ReadOnly] public float Gravity;

    public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
    {
        var transforms = chunk.GetNativeArray(TransformHandle);
        var velocities = chunk.GetNativeArray(VelocityHandle);

        var gravityDt = TimeStep * Gravity; 
        for (int i = 0; i < chunk.Count; ++i)
        {
            var tm = transforms[i];
            var velOld = velocities[i];
            var velNew = velOld.Value;
            velNew.y += gravityDt; 
            var pos = tm.Value.Position;
            pos += TimeStep * velNew;
            // bounce off lower bottom
            pos.y *= math.sign(pos.y);
        }
    }
}

[BurstCompile]
partial struct MovementSystem : ISystem
{
    private EntityQuery Query;
    public ComponentTypeHandle<LocalToWorldTransform> TransformHandle;
    [ReadOnly] public ComponentTypeHandle<Velocity> VelocityHandle;

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        Query = state.GetEntityQuery(typeof(LocalToWorldTransform), typeof(Velocity));
        TransformHandle = state.GetComponentTypeHandle<LocalToWorldTransform>(false);
        VelocityHandle = state.GetComponentTypeHandle<Velocity>(true);
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