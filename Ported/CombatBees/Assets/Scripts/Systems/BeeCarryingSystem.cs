using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;


[UpdateInGroup(typeof(InitializationSystemGroup))]
public class BeeCarrying : SystemBase
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

        Entities.WithoutBurst()
                .ForEach( ( Entity bee, ref Velocity velocity, in Translation translation, in Carrying carrying, in TargetPosition targetPosition, in Speed speed) =>
            {
                //Make the bee move towards the target position
                float3 direction = targetPosition.Value - translation.Value;
                velocity.Value = math.normalize( direction ) * speed.Value;

                //If the bee is close enough, change its state to Carrying
                float d = math.length(direction);
                if(d < 1)
                {
                    ecb.RemoveComponent<Parent>( carrying.Value );
                    ecb.RemoveComponent<LocalToParent>( carrying.Value );
                    ecb.SetComponent<Translation>(carrying.Value, translation);
                    
                    ecb.RemoveComponent<Carrying>( bee );
                    ecb.AddComponent<Idle>( bee );
                }
                
                
            } ).Run();
    }
}
