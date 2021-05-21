using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[UpdateAfter(typeof(BeeUpdateGroup))]
[UpdateBefore(typeof(DestroyerSystem))]
public class GravitySystem : SystemBase
{
    private EntityCommandBufferSystem EntityCommandBufferSystem;
    
    protected override void OnCreate()
    {
        EntityCommandBufferSystem = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
    }
    
    protected override void OnUpdate()
    {
        var ecb = EntityCommandBufferSystem.CreateCommandBuffer().AsParallelWriter();
        
        var time = Time.DeltaTime;
        var gravity = new float3(0, -9.81f, 0);

        Entities
            .WithName("UseGravity")
            .WithAll<HasGravity>()
            .ForEach((int entityInQueryIndex, Entity entity, ref Velocity velocity, ref Translation translation, in NonUniformScale scale) =>
            {
                velocity.Value += gravity * time;

                var halfHeight = scale.Value.y / 2;

                if (translation.Value.y < halfHeight)
                {
                    translation.Value.y = halfHeight;
                    velocity.Value.y = 0;

                    ecb.RemoveComponent<HasGravity>(entityInQueryIndex, entity);
                    ecb.AddComponent<OnCollision>(entityInQueryIndex, entity);
                }
            }).ScheduleParallel();

        EntityCommandBufferSystem.AddJobHandleForProducer(Dependency);
    }
}
