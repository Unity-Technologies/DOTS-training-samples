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
                .WithNone<Velocity>()
                .ForEach( ( Entity bee, ref Translation translation, in TargetEntity targetEntity, in Speed speed) =>
            {
                
                //Make the bee move towards the target entity
                Translation targetEntityTranslationComponent = EntityManager.GetComponentData<Translation>(targetEntity.Value);
                float3 direction = targetEntityTranslationComponent.Value - translation.Value;
                float3 directionNormalized = math.normalize(direction);

                translation.Value += directionNormalized * speed.Value * deltaTime;

                // If resource has been taken in meanwhile
                if (HasComponent<Taken>(targetEntity.Value))
                {
                    ecb.RemoveComponent<Collecting>(bee);
                    ecb.AddComponent<Idle>(bee);
                    ecb.RemoveComponent<TargetEntity>(bee);
                }
                else
                {
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
                        float hiveDistance = battlefield.HiveDistance + 1f;

                        //TODO : take into acount the position of the battlefield (if it's not in 0,0,0)
                        if (HasComponent<TeamA>(bee))
                        {
                            hivePosition = new float3(0, 0, -hiveDistance);
                        }
                        else
                        {
                            hivePosition = new float3(0, 0, hiveDistance);
                        }


                        //Add
                        ecb.AddComponent<TargetPosition>(bee, new TargetPosition { Value = hivePosition });

                        //Remove the collecting tag from the bee
                        ecb.RemoveComponent<Collecting>(bee);

                        //Add the carrying tag to the bee
                        ecb.AddComponent<Carrying>(bee, new Carrying { Value = targetEntity.Value });
                    }
                }
                
            } ).Run();
    }
}
