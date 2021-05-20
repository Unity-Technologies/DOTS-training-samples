using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
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
        var time = Time.DeltaTime;
        var gravity = new float3(0, -9.81f, 0);

        var ecb = EntityCommandBufferSystem.CreateCommandBuffer();

        Entities
            .WithName("UseGravity")
            .WithAll<HasGravity>()
            .ForEach((Entity entity, ref Velocity velocity, ref Translation translation, in NonUniformScale scale) =>
            {
                velocity.Value += gravity * time;

                var halfHeight = scale.Value.y / 2;

                if (translation.Value.y < halfHeight)
                {
                    translation.Value.y = halfHeight;

                    ecb.RemoveComponent<HasGravity>(entity);
                    ecb.AddComponent<OnCollision>(entity);
                    velocity.Value.y = 0;
                }
            }).Schedule();
        
        Entities
            .WithName("SendBeeToDeath")
            .WithAll<IsBee, OnCollision>()
            .WithNone<LifeSpan>()
            .ForEach((Entity entity) =>
            {
                ecb.AddComponent(entity, new LifeSpan { Value = 1 });
            }).Schedule();

        EntityCommandBufferSystem.AddJobHandleForProducer(Dependency);
    }
}
