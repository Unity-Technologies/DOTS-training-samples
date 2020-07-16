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

        var ecb = m_ECBSystem.CreateCommandBuffer().ToConcurrent();

        Entities.ForEach((int entityInQueryIndex, ref Velocity velocity, ref Target target, in Translation pos) =>
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
            var delta = targetPos.Value - pos.Value;
            var sqrDist = math.distancesq(targetPos.Value, pos.Value);
            if (sqrDist > math.pow(attackDistance, 2))
            {
                velocity.Value += delta * (chaseForce * deltaTime / math.sqrt(sqrDist));
            }
            else
            {
                // bee.isAttacking = true;
                velocity.Value += delta * (attackForce * deltaTime / math.sqrt(sqrDist));

                if (sqrDist < math.pow(hitDistance, 2))
                {
                    // Hit on enemy
                    ecb.AddComponent<Dead>(entityInQueryIndex, target.EnemyTarget);
                    // ParticleManager.SpawnParticle(bee.enemyTarget.position,ParticleType.Blood,bee.velocity * .35f,2f,6);
                    // bee.enemyTarget.dead = true;
                    // bee.enemyTarget.velocity *= .5f;
                    // bee.enemyTarget = null;
                }
            }
        }).ScheduleParallel();
    }
}
