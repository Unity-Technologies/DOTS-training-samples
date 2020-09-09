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
                .WithNone<Velocity>()
                .ForEach( ( Entity bee, ref Translation translation, in Carrying carrying, in TargetPosition targetPosition, in Speed speed) =>
            {
                
                //Make the bee move towards the target position
                float3 direction = targetPosition.Value - translation.Value;
                float3 directionNormalized = math.normalize(direction);

                translation.Value += directionNormalized * speed.Value * deltaTime;
                
                //If the bee is close enough, change its state to Carrying
                float d = math.length(direction);
                if(d < 1)
                {

                    ecb.RemoveComponent<Parent>( carrying.Value );
                    ecb.RemoveComponent<LocalToParent>( carrying.Value );

                    ecb.SetComponent<Translation>(carrying.Value, translation);

                    //Remove the collecting tag from the bee
                    ecb.RemoveComponent<Carrying>( bee );

                    //Remove the collecting tag from the bee
                    ecb.AddComponent<Idle>( bee );


                }
                
            } ).Run();
    }
}
