using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;


[UpdateInGroup(typeof(InitializationSystemGroup))]
public class BeeAttackingSystem : SystemBase
{
    private EntityCommandBufferSystem m_CommandBufferSystem;

    private Random m_Random;
    private EntityCommandBufferSystem m_ECBSystem;

    protected override void OnCreate()
    {
        m_CommandBufferSystem = World.GetExistingSystem<EndInitializationEntityCommandBufferSystem>();
    }

    protected override void OnUpdate()
    {
        var ecb = m_CommandBufferSystem.CreateCommandBuffer();

        var deltaTime = Time.DeltaTime;

        Entities.WithAll<Attack>()
                .WithoutBurst()
                .ForEach( ( Entity bee, ref Velocity velocity, in Translation translation, in TargetEntity targetEntity, in Speed speed) =>
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
                    velocity.Value = math.normalize( direction ) * speed.Value;

                    //If the bee is close enough, change its state to Carrying
                    float d = math.length(direction);
                    if (d < 1)
                    {
                        //If the enemy is carrying something, we need to un-parent that "something"
                        if(HasComponent<Carrying>(targetEntity.Value))
                        {
                            ecb.RemoveComponent<Carrying>(targetEntity.Value);

                            var carryingComponentFromEnemy = EntityManager.GetComponentData<Carrying>(targetEntity.Value);
                            ecb.RemoveComponent<Parent>(carryingComponentFromEnemy.Value);
                            ecb.RemoveComponent<LocalToParent>(carryingComponentFromEnemy.Value);

                            ecb.SetComponent<Translation>(carryingComponentFromEnemy.Value, translation);
                        }
                        
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
                            

                        ecb.AddComponent<Velocity>(targetEntity.Value, new Velocity { Value = new float3(0, 0, 0) } );

                        //Switch to Idle
                        ecb.RemoveComponent<TargetEntity>(bee);
                        ecb.RemoveComponent<Attack>(bee);
                        ecb.AddComponent<Idle>(bee);
                    }
                }
                
            } ).Run();

            m_CommandBufferSystem.AddJobHandleForProducer(Dependency);
    }
}
