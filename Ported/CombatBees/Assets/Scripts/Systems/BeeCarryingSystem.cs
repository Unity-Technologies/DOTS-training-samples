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

                //If the bee is close enough, change its state to Carrying
                float d = math.length(direction);
                if(d < 1)
                {
                    // drop the recource
                    ecb.RemoveComponent<Parent>( entityInQueryIndex, carrying.Value );
                    //ecb.RemoveComponent<Taken>( carrying.Value );
                    ecb.RemoveComponent<LocalToParent>( entityInQueryIndex, carrying.Value );
                    ecb.SetComponent<Translation>( entityInQueryIndex, carrying.Value, translation );
                    
                    // revert to idle
                    ecb.RemoveComponent<Carrying>( entityInQueryIndex, bee );
                    ecb.AddComponent<Idle>( entityInQueryIndex, bee );
                }
            } ).ScheduleParallel();
        
        m_CommandBufferSystem.AddJobHandleForProducer( Dependency );
    }
}
