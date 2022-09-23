using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[BurstCompile]
partial struct ResourceCollectingJob : IJobEntity {
    public float DeltaTime;
    public float ChaseForce;
    public float GrabDistanceSquared;

    [ReadOnly] public ComponentLookup<LocalToWorldTransform> transformLookup;

    public EntityCommandBuffer.ParallelWriter ecb;

    void Execute(Entity entity, [EntityInQueryIndex] int index, in TransformAspect prs, ref Velocity velocity, ref TargetId target, ref IsHolding isHolding) {

        if (transformLookup.TryGetComponent(target.Value, out var resourceTransform) && !isHolding.Value)
        {
            float3 delta = resourceTransform.Value.Position - prs.Position;
            float sqrDist = delta.x * delta.x + delta.y * delta.y + delta.z * delta.z;
            if (sqrDist > GrabDistanceSquared)
            {
                velocity.Value += delta * (ChaseForce * DeltaTime / Mathf.Sqrt(sqrDist));
            }
            else
            {
                ecb.SetComponent(index, target.Value, new Holder() { Value = entity });
                isHolding.Value = true;
            }
        }
    }
}
