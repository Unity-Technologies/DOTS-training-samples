using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

public class MoveToTarget : SystemBase
{
    private EntityCommandBufferSystem m_ECBSystem;

    protected override void OnCreate()
    {
        m_ECBSystem = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
    }

    protected override void OnUpdate()
    {
        var deltaTime = Time.DeltaTime;
        var chaseForce = BeeManager.Instance.chaseForce;
        var attackForce = BeeManager.Instance.attackForce;
        var attackDistance = BeeManager.Instance.attackDistance;
        var hitDistance = BeeManager.Instance.hitDistance;
        var rotationStiffness = BeeManager.Instance.rotationStiffness;
        var ecb = m_ECBSystem.CreateCommandBuffer().ToConcurrent();

        Entities
            .WithNone<Dead>()
            .ForEach((Entity entity, int entityInQueryIndex, ref Velocity velocity, ref Smoothing smoothing, ref Target target, in Translation pos) =>
        {
            Translation targetPos;
            if (target.EnemyTarget != Entity.Null)
            {
                targetPos = GetComponent<Translation>(target.EnemyTarget);
            }
            else if (target.ResourceTarget != Entity.Null)
            {
                targetPos = GetComponent<Translation>(target.ResourceTarget);
            }
            else
            {
                return;
            }

            var oldSmoothPos = smoothing.SmoothPosition;

            var delta = targetPos.Value - pos.Value;
            var sqrDist = math.distancesq(targetPos.Value, pos.Value);

            // Not attacking
            if (sqrDist > math.pow(attackDistance, 2))
            {
                velocity.Value += delta * (chaseForce * deltaTime / math.sqrt(sqrDist));
                smoothing.SmoothPosition = math.lerp(smoothing.SmoothPosition, pos.Value, deltaTime * rotationStiffness);
            }

            // Attacking
            else
            {
                smoothing.SmoothPosition = pos.Value;

                velocity.Value += delta * (attackForce * deltaTime / math.sqrt(sqrDist));

                if (sqrDist < math.pow(hitDistance, 2))
                {
                    // Hit on enemy
                    ecb.AddComponent<Dead>(entityInQueryIndex, target.EnemyTarget);
                    ecb.AddComponent<DespawnTimer>(entityInQueryIndex, target.EnemyTarget, new DespawnTimer { Time = 2 });
                    ecb.AddComponent<Gravity>(entityInQueryIndex, target.EnemyTarget);

                    ecb.SetComponent<Target>(entityInQueryIndex, entity, new Target { EnemyTarget = Entity.Null });
                    // ParticleManager.SpawnParticle(bee.enemyTarget.position,ParticleType.Blood,bee.velocity * .35f,2f,6);
                    // bee.enemyTarget.dead = true;
                    // bee.enemyTarget.velocity *= .5f;
                    // bee.enemyTarget = null;
                }
            }

            smoothing.SmoothDirection = smoothing.SmoothPosition - oldSmoothPos;
        }).ScheduleParallel();
    }
}
