using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;


[UpdateBefore(typeof(MovementSystem))]
[UpdateBefore(typeof(BeeAttackingSystem))]
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

        BattleField battlefield = GetSingleton<BattleField>();

        Entities.WithAll<Collecting>()
                .WithoutBurst()
                .ForEach( ( Entity bee, ref Velocity velocity, in Translation translation, in TargetEntity targetEntity, in Speed speed) =>
            {
                // check if the resource has been destroyed // If resource has been taken by another bee
                if (!HasComponent<Rotation>(targetEntity.Value) || HasComponent<Taken>(targetEntity.Value))
                {
                    ecb.RemoveComponent<Collecting>(bee);
                    ecb.RemoveComponent<TargetEntity>(bee);
                    ecb.AddComponent<Idle>(bee);
                    return;
                }

                //If the bee is close enough, change its state to Carrying
                Translation targetEntityTranslationComponent = GetComponent<Translation>( targetEntity.Value );
                float distanceToResource = math.length(targetEntityTranslationComponent.Value - translation.Value);
                if (distanceToResource < 1)
                {
                    ecb.AddComponent<Parent>(targetEntity.Value, new Parent { Value = bee });
                    ecb.AddComponent<Taken>(targetEntity.Value);
                    ecb.AddComponent<LocalToParent>(targetEntity.Value);
                    ecb.SetComponent<Translation>(targetEntity.Value, new Translation { Value = new float3(0, -1, 0) });

                    
                    float hiveDistance = battlefield.HiveDistance + 10f;
                    float3 hivePosition = new float3(0, 0, hiveDistance);
                    if (HasComponent<TeamA>(bee))
                        hivePosition.z *= -1;

                    ecb.RemoveComponent<Collecting>(bee);
                    ecb.RemoveComponent<TargetEntity>(bee);
                    ecb.SetComponent<TargetPosition>(bee, new TargetPosition { Value = hivePosition });
                    ecb.AddComponent<Carrying>(bee, new Carrying { Value = targetEntity.Value });
                }
                
            } ).Run();
    }
}
