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
        var ecb = m_CommandBufferSystem.CreateCommandBuffer();

        var b = GetSingleton<BattleField>();
        var deltaTime = Time.DeltaTime;

        BattleField battlefield = GetSingleton<BattleField>();

        Entities.WithAll<Attack>()
                .WithoutBurst()
                .ForEach( ( Entity bee, ref Velocity velocity, ref TargetPosition targetPosition, in Translation translation, in TargetEntity targetEntity, in Speed speed) =>
            {
                //If the target bee is dying, agonizing or Destroyed (does not have translation component), back to idle
                if (HasComponent<Dying>(targetEntity.Value) || HasComponent<Agony>(targetEntity.Value) || !HasComponent<Rotation>(targetEntity.Value))
                {
                    ecb.RemoveComponent<Attack>(bee);
                    //ecb.RemoveComponent<TargetEntity>(bee);
                    ecb.AddComponent<Idle>(bee);
                }
                else
                {
                    //Make the bee move towards the target entity
                    Translation targetEntityTranslationComponent = EntityManager.GetComponentData<Translation>(targetEntity.Value);
                    float3 direction = targetEntityTranslationComponent.Value - translation.Value;
                    //targetPosition.Value = targetEntityTranslationComponent.Value;

                    //If the bee is close enough, kill the other bee
                    float d = math.length(direction);
                    if (d < 1)
                    {
                        //If the enemy is carrying something, we need to un-parent that "something"
                        if(HasComponent<Carrying>(targetEntity.Value))
                        {
                            ecb.RemoveComponent<Carrying>(targetEntity.Value);

                            var carryingComponentFromEnemy = EntityManager.GetComponentData<Carrying>(targetEntity.Value);
                            ecb.SetComponent<Parent>(carryingComponentFromEnemy.Value, new Parent { Value = bee });

                            //Take over the resource

                            float3 hivePosition;
                            float hiveDistance = battlefield.HiveDistance + 1f;

                            //TODO : take into acount the position of the battlefield (if it's not in 0,0,0)
                            if (HasComponent<TeamA>(bee))
                                hivePosition = new float3(0, 0, -hiveDistance);
                            else
                                hivePosition = new float3(0, 0, hiveDistance);

                            ecb.RemoveComponent<TargetEntity>(bee);
                            ecb.RemoveComponent<Attack>(bee);
                            ecb.AddComponent<Carrying>(bee, new Carrying { Value = carryingComponentFromEnemy.Value });
                            ecb.SetComponent<TargetPosition>(bee, new TargetPosition { Value = hivePosition });
                        }
                        else
                        {
                            //Switch to Idle
                            ecb.RemoveComponent<TargetEntity>(bee);
                            ecb.RemoveComponent<Attack>(bee);
                            ecb.AddComponent<Idle>(bee);
                        }

                        var bloodSpawner = ecb.Instantiate(b.BloodSpawner);
                        ecb.SetComponent<Translation>(bloodSpawner, translation);
                        
                        ecb.AddComponent<Dying>(targetEntity.Value);
                        
                        // TODO Figure out a better way of doing this
                        if(HasComponent<Idle>(targetEntity.Value))
                            ecb.RemoveComponent<Idle>(targetEntity.Value);
                        if(HasComponent<Attack>(targetEntity.Value))
                            ecb.RemoveComponent<Attack>(targetEntity.Value);
                        if(HasComponent<Carrying>(targetEntity.Value))
                            ecb.RemoveComponent<Carrying>(targetEntity.Value);
                        if(HasComponent<Collecting>(targetEntity.Value))
                            ecb.RemoveComponent<Collecting>(targetEntity.Value);
                    }
                    else if( d < 10 )
                    {
                        // sprint towards the 
                        velocity.Value += direction * deltaTime * 20;
                    }
                }
                
            } ).Run();

            m_CommandBufferSystem.AddJobHandleForProducer(Dependency);
    }
}
