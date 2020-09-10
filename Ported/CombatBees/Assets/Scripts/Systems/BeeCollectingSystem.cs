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

        BattleField battlefield = GetSingleton<BattleField>();

        Entities.WithAll<Collecting>()
                .WithoutBurst()
                .ForEach( ( Entity bee, ref Velocity velocity, in Translation translation, in TargetEntity targetEntity, in Speed speed) =>
            {
                if (!HasComponent<Rotation>(targetEntity.Value))
                {
                    //Remove the collecting tag from the bee
                    ecb.RemoveComponent<Collecting>(bee);

                    //Remove the collecting tag from the bee
                    ecb.RemoveComponent<TargetEntity>(bee);

                    //Add the carrying tag to the bee
                    ecb.AddComponent<Idle>(bee);

                    return;
                }

                // If resource has been taken in meanwhile
                if (HasComponent<Taken>(targetEntity.Value))
                {
                    ecb.RemoveComponent<Collecting>(bee);
                    ecb.AddComponent<Idle>(bee);
                    ecb.RemoveComponent<TargetEntity>(bee);
                }
                else
                {
                    //Make the bee move towards the target entity
                    // TODO figure out how to replace EntityManager
                    Translation targetEntityTranslationComponent = EntityManager.GetComponentData<Translation>(targetEntity.Value);
                    float3 direction = targetEntityTranslationComponent.Value - translation.Value;

                    velocity.Value = math.normalize( direction ) * speed.Value;

                    //If the bee is close enough, change its state to Carrying
                    float d = math.length(direction);
                    if (d < 1)
                    {
                        ecb.AddComponent<Parent>(targetEntity.Value, new Parent { Value = bee });
                        ecb.AddComponent<Taken>(targetEntity.Value);
                        ecb.AddComponent<LocalToParent>(targetEntity.Value);
                        ecb.SetComponent<Translation>(targetEntity.Value, new Translation { Value = new float3(0, -1, 0) });

                        //Remove the target entity
                        ecb.RemoveComponent<TargetEntity>(bee);

                        float3 hivePosition;
                        float hiveDistance = battlefield.HiveDistance + 10f;

                        //TODO : take into acount the position of the battlefield (if it's not in 0,0,0)
                        if (HasComponent<TeamA>(bee))
                            hivePosition = new float3(0, 0, -hiveDistance);
                        else
                            hivePosition = new float3(0, 0, hiveDistance);
                        
                        ecb.AddComponent<TargetPosition>(bee, new TargetPosition { Value = hivePosition });
                        ecb.RemoveComponent<Collecting>(bee);
                        ecb.AddComponent<Carrying>(bee, new Carrying { Value = targetEntity.Value });
                    }
                }
                
            } ).Run();
    }
}
