using System;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[BurstCompile]
[WithAny(typeof(YellowTeam), typeof(BlueTeam))]
[WithNone(typeof(Falling))]
partial struct BeeClampingJob : IJobEntity {
    public float3 fieldBounds;
    [ReadOnly] public ComponentLookup<UniformScale> scaleLookup;
    [ReadOnly] public ComponentLookup<TargetId> targetIDLookup;

    void Execute(Entity entity, ref TransformAspect prs, ref Velocity velocityComponent)
    {
        var localToWorld = prs.LocalToWorld; 
        ref var velocity = ref velocityComponent.Value; 
        if (Math.Abs(localToWorld.Position.x) > fieldBounds.x * .5f) {
            localToWorld.Position.x = (fieldBounds.x * .5f) * Mathf.Sign(localToWorld.Position.x);
            velocity.x *= -.5f;
            velocity.y *= .8f;
            velocity.z *= .8f;
        }
        if (Math.Abs(localToWorld.Position.z) > fieldBounds.z * .5f) {
            localToWorld.Position.z = (fieldBounds.z * .5f) * Mathf.Sign(localToWorld.Position.z);
            velocity.z *= -.5f;
            velocity.x *= .8f;
            velocity.y *= .8f;
        }
        float resourceModifier = 0f;
        if (targetIDLookup.TryGetComponent(entity, out TargetId targetIdComponent)) {
            if (scaleLookup.TryGetComponent(targetIdComponent.Value, out UniformScale resourceSize)) {
                resourceModifier = resourceSize.Value;   
            }
        }
        if (Math.Abs(localToWorld.Position.y) > fieldBounds.y * .5f - resourceModifier) {
            localToWorld.Position.y = (fieldBounds.y * .5f - resourceModifier) * Mathf.Sign(localToWorld.Position.y);
            velocity.y *= -.5f;
            velocity.z *= .8f;
            velocity.x *= .8f;
        }

        prs.LocalToWorld = localToWorld;
    }
}