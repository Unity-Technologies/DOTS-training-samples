using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[BurstCompile]
[WithNone(typeof(DecayTimer))]
partial struct AttackingJob : IJobEntity {
    public float deltaTime;
    public float chaseForce;
    public float attackForce;
    public float attackDistanceSquared;
    public float hitDistanceSquared;

    [ReadOnly] public ComponentLookup<LocalToWorldTransform> transformLookup;

    // [ReadOnly] public ComponentLookup<Velocity> velocityLookup;
    public EntityCommandBuffer.ParallelWriter ecb;

    void Execute(Entity entity, [EntityInQueryIndex] int index, in TransformAspect prs, ref Velocity velocity, ref TargetId target, ref IsAttacking isAttacking) {
        isAttacking.Value = false;
        if (transformLookup.TryGetComponent(target.Value, out var enemyPosition)) {
            float3 delta = enemyPosition.Value.Position - prs.Position;
            float sqrDist = delta.x * delta.x + delta.y * delta.y + delta.z * delta.z;
            if (sqrDist > attackDistanceSquared) {
                velocity.Value += delta * (chaseForce * deltaTime / Mathf.Sqrt(sqrDist));
            } else {
                isAttacking.Value = true;
                velocity.Value += delta * (attackForce * deltaTime / Mathf.Sqrt(sqrDist));
                if (sqrDist < hitDistanceSquared) {
                    // TODO: spawn blood
                    // ParticleManager.SpawnParticle(bee.enemyTarget.position,ParticleType.Blood,bee.velocity * .35f,2f,6);
                    ecb.AddComponent(index, target.Value, new DecayTimer() {Value = 1f});
                    ecb.SetComponentEnabled<Falling>(index, target.Value, true);
                    ecb.SetComponentEnabled<IsAttacking>(index, entity, false);
                    ecb.SetComponentEnabled<TargetId>(index, entity, false);
                    target.Value = Entity.Null;
                }
            }
        }
    }
}