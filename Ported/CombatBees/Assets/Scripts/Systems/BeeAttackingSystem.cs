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
                .WithNone<Velocity>()
                .WithoutBurst()
                .ForEach( ( Entity bee, ref Translation translation, in TargetEntity targetEntity, in Speed speed) =>
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
                    float3 directionNormalized = math.normalize(direction);

                    translation.Value += directionNormalized * speed.Value * deltaTime;

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

                        ecb.AddComponent<Velocity>(targetEntity.Value, new Velocity { Value = new float3(0, 0, 0) } );

                        //Remove the target entity
                        ecb.RemoveComponent<TargetEntity>(bee);

                        //Remove attacking component
                        ecb.RemoveComponent<Attack>(bee);

                        //Switch to Idle
                        ecb.AddComponent<Idle>(bee);

                    }
                }
                
            } ).Run();
    }
}
