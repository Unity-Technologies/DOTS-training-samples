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
                .ForEach( ( Entity bee, ref Translation translation, in TargetEntity targetEntity, in Speed speed) =>
            {

                //Make the bee move towards the target entity
                Translation targetEntityTranslationComponent = EntityManager.GetComponentData<Translation>(targetEntity.Value);
                float3 direction = targetEntityTranslationComponent.Value - translation.Value;
                float3 directionNormalized = math.normalize(direction);

                translation.Value += directionNormalized * speed.Value * deltaTime;

                if (HasComponent<Dying>(targetEntity.Value))
                {
                    ecb.RemoveComponent<Attack>(bee);
                    ecb.RemoveComponent<TargetEntity>(bee);
                    ecb.AddComponent<Idle>(bee);
                }
                else
                {

                    //If the bee is close enough, change its state to Carrying
                    float d = math.length(direction);
                    if (d < 1)
                    {
                        // TODO: remember to override systems to use bees without dying for their actions
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
