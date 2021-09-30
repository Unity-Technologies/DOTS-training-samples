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
        var bounds = GetSingleton<WorldBounds>();

        Entities
            .WithAll<Particle>()
            .WithNone<Grounded>()
            .ForEach((Entity entity, int entityInQueryIndex, ref Translation translation, ref Velocity velocity) =>
            {
                velocity.Value = velocity.Value + (Gravity * deltaTime);
                
                float3 position = translation.Value + (velocity.Value * deltaTime);
                translation.Value = WorldUtils.ClampToWorldBounds(bounds, position, 0.1f);

                if (translation.Value.y <= 0.1f)
                {
                    translation.Value.y = 0.01f;
                    ecb.AddComponent(entityInQueryIndex, entity, new Grounded{});
                }
            }).ScheduleParallel();
        
        system.AddJobHandleForProducer(Dependency);
    }
}
