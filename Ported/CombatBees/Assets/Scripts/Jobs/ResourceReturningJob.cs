using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[BurstCompile]
partial struct ResourceReturningJob : IJobEntity {
    public float DeltaTime;
    public float CarryForce;
    public float3 FieldSize;
    public BeeTeam Hive;

    [ReadOnly] public ComponentLookup<Holder> holderLookup;

    public EntityCommandBuffer.ParallelWriter ecb;

    void Execute(Entity entity, [EntityInQueryIndex] int index, in TransformAspect prs, ref Velocity velocity, ref TargetId target, ref IsHolding isHolding) {

        if (isHolding.Value)
        {
            var localToWorld = prs.LocalToWorld;
            float3 spawnSide = Hive == BeeTeam.Blue ? -localToWorld.Right() : localToWorld.Right();
            float3 targetPos = spawnSide * (-FieldSize.x * .45f + FieldSize.x * .9f);
            targetPos.y = 0;
            targetPos.z = prs.Position.z;
            float3 delta = targetPos - prs.Position;
            float dist = Mathf.Sqrt(delta.x * delta.x + delta.y * delta.y + delta.z * delta.z);
            velocity.Value += (targetPos - prs.Position) * (CarryForce * DeltaTime / dist);

            if (dist < 1f)
            {
                isHolding.Value = false;
                ecb.SetComponentEnabled<IsHolding>(index, entity, false);
                // target.Value can be a deleted entity
                if (holderLookup.TryGetComponent(target.Value, out var holder) && holder.Value == entity) {
                    // TODO: do drop resource
                    ecb.SetComponent(index, target.Value, new Holder() {Value = Entity.Null});
                    ecb.AddComponent<Falling>(index, target.Value);
                    ecb.SetComponentEnabled<Falling>(index, target.Value, true);
                }
                target.Value = Entity.Null;
            }
        }
    }
}
