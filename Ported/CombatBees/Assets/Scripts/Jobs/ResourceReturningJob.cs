﻿using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[BurstCompile]
partial struct ResourceReturningJob : IJobEntity {
    public float DeltaTime;
    public float CarryForce;
    public BeeTeam Hive;

    public EntityCommandBuffer.ParallelWriter ecb;

    void Execute(Entity entity, [EntityInQueryIndex] int index, in TransformAspect prs, ref Velocity velocity, ref TargetId target, ref IsHolding isHolding) {

        if (isHolding.Value)
        {
            var localToWorld = prs.LocalToWorld;
            float3 spawnSide = Hive == BeeTeam.Blue ? localToWorld.Right() : -localToWorld.Right();
            float3 targetPos = spawnSide * (-Field.size.x * .45f + Field.size.x * .9f);
            targetPos.y = 0;
            targetPos.z = prs.Position.z;
            float3 delta = targetPos - prs.Position;
            float dist = Mathf.Sqrt(delta.x * delta.x + delta.y * delta.y + delta.z * delta.z);
            velocity.Value += (targetPos - prs.Position) * (CarryForce * DeltaTime / dist);

            if (dist < 5f)
            {
                target.Value = Entity.Null;
                isHolding.Value = false;
                ecb.SetComponentEnabled<IsHolding>(index, entity, false);
                // TODO: do drop resource
            }
        }
    }
}
