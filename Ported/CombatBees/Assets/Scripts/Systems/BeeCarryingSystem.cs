using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;


[UpdateBefore(typeof(BeeAttackingSystem))]
public class BeeCarrying : SystemBase
{
    private EntityCommandBufferSystem m_CommandBufferSystem;

    protected override void OnCreate()
    {
        m_CommandBufferSystem = World.GetExistingSystem<EndInitializationEntityCommandBufferSystem>();
    }

    protected override void OnUpdate()
    {
        var ecb = m_CommandBufferSystem.CreateCommandBuffer().AsParallelWriter();

        Entities
                .ForEach( ( int entityInQueryIndex, Entity bee, in Translation translation, in Carrying carrying, in TargetPosition targetPosition ) =>
            {
                //Make the bee move towards the target position
                float3 direction = targetPosition.Value - translation.Value;
                ecb.SetComponent<Translation>( entityInQueryIndex, carrying.Value, new Translation { Value = new float3( translation.Value.x, translation.Value.y-1, translation.Value.z )} );
                ecb.SetComponent<Velocity>( entityInQueryIndex, carrying.Value, new Velocity{ Value = float3.zero} );
                
                //If the bee is close enough, change its state to Carrying
                float d = math.length(direction);
                if(d < 1)
                {
                    // drop the recource
                    //ecb.RemoveComponent<Parent>( entityInQueryIndex, carrying.Value );
                    ecb.AddComponent<Delivered>( entityInQueryIndex, carrying.Value );
                    ecb.SetComponent<Translation>( entityInQueryIndex, carrying.Value, translation );
                    
                    // revert to idle
                    ecb.RemoveComponent<Carrying>( entityInQueryIndex, bee );
                    ecb.AddComponent<Idle>( entityInQueryIndex, bee );
                }
            } ).ScheduleParallel();
        
        m_CommandBufferSystem.AddJobHandleForProducer( Dependency );
    }
}
