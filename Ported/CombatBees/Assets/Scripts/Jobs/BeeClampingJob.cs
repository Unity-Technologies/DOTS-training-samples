using System;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

[BurstCompile]
[WithAll(typeof(Team))]
partial struct BeeClampingJob : IJobEntity {
    public float3 fieldBounds;
    public ComponentLookup<UniformScale> scaleLookup;
    public ComponentLookup<IsHolding> hasHolding;

    void Execute(Entity entity, ref Position positionComponent, ref Velocity velocityComponent, in TargetId targetIdComponent) {
        ref var position = ref positionComponent.Value; 
        ref var velocity = ref velocityComponent.Value; 
        if (Math.Abs(position.x) > fieldBounds.x * .5f) {
            position.x = (fieldBounds.x * .5f) * Mathf.Sign(position.x);
            velocity.x *= -.5f;
            velocity.y *= .8f;
            velocity.z *= .8f;
        }
        if (Math.Abs(position.z) > fieldBounds.z * .5f) {
            position.z = (fieldBounds.z * .5f) * Mathf.Sign(position.z);
            velocity.z *= -.5f;
            velocity.x *= .8f;
            velocity.y *= .8f;
        }
        float resourceModifier = 0f;
        if (hasHolding.HasComponent(entity)) {
            if (scaleLookup.TryGetComponent(targetIdComponent.Value, out UniformScale resourceSize)) {
                resourceModifier = resourceSize.Value;   
            }
        }
        if (Math.Abs(position.y) > fieldBounds.y * .5f - resourceModifier) {
            position.y = (fieldBounds.y * .5f - resourceModifier) * Mathf.Sign(position.y);
            velocity.y *= -.5f;
            velocity.z *= .8f;
            velocity.x *= .8f;
        }
    }
}