using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;


[UpdateInGroup(typeof(InitializationSystemGroup))]
public class BeeCollectingSystem : SystemBase
{
    private EntityCommandBufferSystem m_CommandBufferSystem;

    protected override void OnCreate()
    {
        m_CommandBufferSystem = World.GetExistingSystem<EndInitializationEntityCommandBufferSystem>();
    }

    protected override void OnUpdate()
    {
        var ecb = m_CommandBufferSystem.CreateCommandBuffer();

        var deltaTime = Time.DeltaTime;

        Entities.WithAll<Collecting>()
                .WithoutBurst()
                .ForEach( ( Entity bee, ref Translation translation, in TargetEntity targetEntity, in Speed speed) =>
            {
                
                //Make the bee move towards the target entity
                Translation targetEntityTranslationComponent = EntityManager.GetComponentData<Translation>(targetEntity.Value);
                float3 direction = math.normalize(targetEntityTranslationComponent.Value - translation.Value);

                translation.Value += direction * speed.Value * deltaTime;
                
                //If the bee is close enough, change its state to Carrying

                
            } ).Run();
    }
}
