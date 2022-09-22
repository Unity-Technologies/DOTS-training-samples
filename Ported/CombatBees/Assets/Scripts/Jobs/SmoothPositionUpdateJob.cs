using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[BurstCompile]
partial struct SmoothPositionUpdateJob : IJobEntity {

    public float rotationAmount;
    [ReadOnly] public ComponentLookup<IsAttacking> attackingLookup;

    void Execute(Entity entity, ref SmoothPosition smoothPosition, ref SmoothDirection smoothDirection, in TransformAspect prs) {
        float3 oldSmoothPos = smoothPosition.Value;
        if (attackingLookup.TryGetComponent(entity, out var isAttacking) && isAttacking.Value) {
            smoothPosition.Value = prs.Position;
        } else {
            smoothPosition.Value = Lerp(smoothPosition.Value, prs.Position, rotationAmount);
        }

        smoothDirection.Value = smoothPosition.Value - oldSmoothPos;
    }

    static float3 Lerp(float3 a, float3 b, float t) {
        t = Mathf.Clamp01(t);
        return new float3(a.x + (b.x - a.x) * t, a.y + (b.y - a.y) * t, a.z + (b.z - a.z) * t);
    }
}