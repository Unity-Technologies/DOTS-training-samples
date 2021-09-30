using Unity.Entities;
using Unity.Collections;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Rendering;
using UnityEngine;

public partial class ParticleMovementSystem : SystemBase
{
    private static readonly float3 Gravity = new float3(0.0f, -9.8f, 0.0f);

    protected override void OnUpdate()
    {
        float deltaTime = Time.DeltaTime;
        var system = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
        var ecb = system.CreateCommandBuffer().AsParallelWriter();
        
        Entities
            .WithAll<Particle>()
            .WithNone<Grounded>()
            .ForEach((Entity entity, int entityInQueryIndex, ref Translation translation, ref Velocity velocity) =>
            {
                velocity.Value = velocity.Value + (Gravity * deltaTime);
                translation.Value = translation.Value + (velocity.Value * deltaTime);
                
                if (translation.Value.y <= 0.0f)
                {
                    translation.Value.y = 0.0f;
                    ecb.AddComponent(entityInQueryIndex, entity, new Grounded{});
                }
            }).ScheduleParallel();
        
        system.AddJobHandleForProducer(Dependency);
    }
}
