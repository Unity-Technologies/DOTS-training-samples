using System;
using Unity.Burst;
using Unity.Collections;
using Unity.Burst.Intrinsics;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.Rendering;

[BurstCompile]
public struct MovementJob : IJobChunk
{
    public ComponentTypeHandle<LocalToWorldTransform> TransformHandle;
    public ComponentTypeHandle<Velocity> VelocityHandle;

    [ReadOnly] public float TimeStep;
    [ReadOnly] public float Gravity;
    [ReadOnly] public Area FieldArea;

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
            bool notOnGround = pos.y < FieldArea.Value.Min.y;

            if (notOnGround && velNew.y < -k_DeepImpactVel) // deep impact
            {
                velNew.y = -velNew.y * k_Restitution;
                velNew.xz *= k_FrictionDamping;
            }
            else
            {
                velNew = math.select(velNew,float3.zero, notOnGround);
            }

            // clamp to field
            pos = math.clamp(pos, FieldArea.Value.Min, FieldArea.Value.Max);

            tmComp.Value.Position = pos;
            velComp.Value = velNew;

            // derive heading of velocity
            var heading = math.atan2(velNew.x, velNew.z);
            var headingRot = quaternion.AxisAngle(math.up(),math.degrees(heading));

            // derive pitch of velocity, by bringing velocity into local space of object (with z forward) and
            // calculating the angle of the velocity in the local yz plane of the object
            var velForwardLocal = math.rotate(math.inverse(headingRot), velNew);
            var pitch = math.atan2(velForwardLocal.y, velForwardLocal.z);
            var pitchRot = quaternion.AxisAngle(math.left(),math.degrees(pitch));

            // final rotation by applying pitch first and then heading (right to left) 
            tmComp.Value.Rotation = math.mul(headingRot, pitchRot);

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
        
        state.RequireForUpdate<BeeConfig>();
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var configEntity = SystemAPI.GetSingletonEntity<BeeConfig>();
        var fieldArea = SystemAPI.GetComponent<Area>(configEntity);
        
        TransformHandle.Update(ref state);
        VelocityHandle.Update(ref state);
        var job = new MovementJob()
        {
            TransformHandle = TransformHandle,
            VelocityHandle = VelocityHandle,
            TimeStep = state.Time.DeltaTime,
            Gravity = -9.81f,
            FieldArea = fieldArea,
        };

        state.Dependency = job.ScheduleParallel(Query, state.Dependency);
    }
}