using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[BurstCompile]
[WithNone(typeof(Decay), typeof(Falling))]
partial struct ResourceHoldingJob : IJobEntity
{
    public float DeltaTime;
    public float HolderSize;
    public float CarryStiffness;

    [ReadOnly] public ComponentLookup<LocalToWorldTransform> TransformLookup;

    public EntityCommandBuffer.ParallelWriter ecb;

    void Execute(Entity entity, [EntityInQueryIndex] int index, in TransformAspect prs, ref Velocity velocity, in Holder holder)
    {
        if (TransformLookup.TryGetComponent(holder.Value, out var holderTransform) && holder.Value != Entity.Null)
        {
            float3 targetPos = holderTransform.Value.Position - new float3(0, 1, 0) * (holderTransform.Value.Scale + HolderSize) * .5f;
            float3 lerpPos = math.lerp(prs.Position, targetPos, CarryStiffness * DeltaTime);
            var scaleTransform = UniformScaleTransform.FromPosition(lerpPos);

            ecb.SetComponent(index, entity, new LocalToWorldTransform() { Value = scaleTransform });

            velocity.Value = 0.0f;
        }
    }
}
