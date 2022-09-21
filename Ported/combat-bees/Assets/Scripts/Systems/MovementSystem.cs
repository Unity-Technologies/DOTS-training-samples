using System;
using Unity.Burst;
using Unity.Collections;
using Unity.Burst.Intrinsics;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
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
            var pos = tmComp.Value.Position;
            float k_DeepImpactVel = 0.5f;
            float k_Restitution = 0.4f;
            float k_FrictionDamping = 0.3f;

            velNew.y += gravityDt;
            pos += TimeStep * velNew;
            float notOnGround = Convert.ToSingle(pos.y > 0);

            if (notOnGround != 1.0f && velNew.y < -k_DeepImpactVel) // deep impact
            {
                velNew.y = -velNew.y * k_Restitution;
                velNew.x *= k_FrictionDamping;
                velNew.z *= k_FrictionDamping;
            }
            else
            {
                velNew.y *= notOnGround;
                velNew.x *= notOnGround;
                velNew.z *= notOnGround;
            }

            // stop at lower bottom
            pos.y *= notOnGround;

            tmComp.Value.Position = pos;
            velComp.Value = velNew;

            // derive heading of velocity
            var heading = math.atan2(velNew.x, velNew.z);
            var headingRot = Quaternion.AngleAxis(math.degrees(heading), Vector3.up);

            // derive pitch of velocity, by bringing velocity into local space of object (with z forward) and
            // calculating the angle of the velocity in the local yz plane of the object
            var velForwardLocal = math.rotate(math.inverse(headingRot), velNew);
            var pitch = math.atan2(velForwardLocal.y, velForwardLocal.z);
            var pitchRot = Quaternion.AngleAxis(math.degrees(pitch), Vector3.left);

            // final rotation by applying pitch first and then heading (right to left) 
            tmComp.Value.Rotation = headingRot * pitchRot;

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