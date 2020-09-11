using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;


[UpdateBefore(typeof(BeeAttackingSystem))]
public class BeeCarrying : SystemBase
{
    protected override void OnUpdate()
    {
        var ecb = new EntityCommandBuffer(Allocator.TempJob);

        Entities.WithoutBurst()
                .ForEach( ( Entity bee, ref Velocity velocity, in Translation translation, in Carrying carrying, in TargetPosition targetPosition, in Speed speed) =>
            {
                //Make the bee move towards the target position
                float3 direction = targetPosition.Value - translation.Value;

                //If the bee is close enough, change its state to Carrying
                float d = math.length(direction);
                if(d < 1)
                {
                    // drop the recource
                    ecb.RemoveComponent<Parent>( carrying.Value );
                    //ecb.RemoveComponent<Taken>( carrying.Value );
                    ecb.RemoveComponent<LocalToParent>( carrying.Value );
                    ecb.SetComponent<Translation>( carrying.Value, translation );
                    
                    // revert to idle
                    ecb.RemoveComponent<Carrying>( bee );
                    ecb.AddComponent<Idle>( bee );
                }
            } ).Run();
        
        ecb.Playback( EntityManager );
        ecb.Dispose();
    }
}
