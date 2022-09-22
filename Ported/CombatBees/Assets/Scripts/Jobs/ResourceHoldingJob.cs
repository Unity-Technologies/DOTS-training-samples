using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[BurstCompile]
partial struct ResourceHoldingJob : IJobEntity
{
    public float DeltaTime;
    public float HolderSize;
    public float CarryStiffness;

    [ReadOnly] public ComponentLookup<LocalToWorldTransform> TransformLookup;

    void Execute(ref TransformAspect prs, ref Velocity velocity, in Holder holder)
    {
        Debug.Log("resource holding");
        
        if (TransformLookup.TryGetComponent(holder.Value, out var holderTransform) && holder.Value != Entity.Null)
        {
            Debug.Log("do holding");
            float3 targetPos = holderTransform.Value.Position - new float3(0, 1, 0) * (holderTransform.Value.Scale + HolderSize) * .5f;
            prs.Position = math.lerp(prs.Position, targetPos, CarryStiffness * DeltaTime);
            velocity.Value = 0.0f;
        }
    }
}
