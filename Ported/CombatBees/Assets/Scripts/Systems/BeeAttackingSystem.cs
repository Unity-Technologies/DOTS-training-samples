using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[UpdateBefore(typeof(MovementSystem))]
public class BeeAttackingSystem : SystemBase
{
    private EntityCommandBufferSystem m_CommandBufferSystem;

    protected override void OnCreate()
    {
        m_CommandBufferSystem = World.GetExistingSystem<EndInitializationEntityCommandBufferSystem>();
    }

    protected override void OnUpdate()
    {
        var ecb = m_CommandBufferSystem.CreateCommandBuffer().AsParallelWriter();

        var b = GetSingleton<BattleField>();
        var deltaTime = Time.DeltaTime;

        BattleField battlefield = GetSingleton<BattleField>();

        Entities.WithAll<Attack>()
                .ForEach( ( int entityInQueryIndex, Entity bee, ref Velocity velocity, ref TargetPosition targetPosition, in Translation translation, in TargetEntity targetEntity) =>
            {
                //If the target bee is dying, agonizing or Destroyed (does not have translation component), back to idle
                if (HasComponent<Dying>(targetEntity.Value) || HasComponent<Agony>(targetEntity.Value) || !HasComponent<Rotation>(targetEntity.Value))
                {
                    ecb.RemoveComponent<Attack>(entityInQueryIndex, bee);
                    //ecb.RemoveComponent<TargetEntity>(bee);
                    ecb.AddComponent<Idle>(entityInQueryIndex, bee);
                }
                else
                {
                    //Make the bee move towards the target entity
                    Translation targetEntityTranslationComponent = GetComponent<Translation>(targetEntity.Value);
                    float3 direction = targetEntityTranslationComponent.Value - translation.Value;
                    targetPosition.Value = targetEntityTranslationComponent.Value;

                    //If the bee is close enough, kill the other bee
                    float d = math.length(direction);
                    if (d < 1)
                    {
                        //If the enemy is carrying something, we need to un-parent that "something"
                        if(HasComponent<Carrying>(targetEntity.Value))
                        {
                            ecb.RemoveComponent<Carrying>(entityInQueryIndex, targetEntity.Value);

                            var carryingComponentFromEnemy = GetComponent<Carrying>(targetEntity.Value);
                            ecb.SetComponent<Parent>(entityInQueryIndex, carryingComponentFromEnemy.Value, new Parent { Value = bee });

                            //Take over the resource

                            float3 hivePosition;
                            float hiveDistance = battlefield.HiveDistance + 1f;

                            //TODO : take into acount the position of the battlefield (if it's not in 0,0,0)
                            if (HasComponent<TeamA>(bee))
                                hivePosition = new float3(0, 0, -hiveDistance);
                            else
                                hivePosition = new float3(0, 0, hiveDistance);

                            ecb.RemoveComponent<TargetEntity>(entityInQueryIndex, bee);
                            ecb.RemoveComponent<Attack>(entityInQueryIndex, bee);
                            ecb.AddComponent<Carrying>(entityInQueryIndex, bee, new Carrying { Value = carryingComponentFromEnemy.Value });
                            ecb.SetComponent<TargetPosition>(entityInQueryIndex, bee, new TargetPosition { Value = hivePosition });
                        }
                        else
                        {
                            //Switch to Idle
                            ecb.RemoveComponent<TargetEntity>(entityInQueryIndex, bee);
                            ecb.RemoveComponent<Attack>(entityInQueryIndex, bee);
                            ecb.AddComponent<Idle>(entityInQueryIndex, bee);
                        }

                        var bloodSpawner = ecb.Instantiate(entityInQueryIndex, b.BloodSpawner);
                        ecb.SetComponent<Translation>(entityInQueryIndex, bloodSpawner, translation);
                        
                        ecb.AddComponent<Dying>(entityInQueryIndex, targetEntity.Value);
                        
                        // TODO Figure out a better way of doing this
                        if(HasComponent<Idle>(targetEntity.Value))
                            ecb.RemoveComponent<Idle>(entityInQueryIndex, targetEntity.Value);
                        if(HasComponent<Attack>(targetEntity.Value))
                            ecb.RemoveComponent<Attack>(entityInQueryIndex, targetEntity.Value);
                        if(HasComponent<Carrying>(targetEntity.Value))
                            ecb.RemoveComponent<Carrying>(entityInQueryIndex, targetEntity.Value);
                        if(HasComponent<Collecting>(targetEntity.Value))
                            ecb.RemoveComponent<Collecting>(entityInQueryIndex, targetEntity.Value);
                    }
                    else if( d < 10 )
                    {
                        // sprint towards the 
                        velocity.Value += direction * deltaTime * 20;
                    }
                }
                
            } ).ScheduleParallel();

            m_CommandBufferSystem.AddJobHandleForProducer(Dependency);
    }
}
