using Unity.Entities;
using Unity.Rendering;
using Unity.Transforms;
using Unity.Mathematics;
using UnityEngine;

public class BloodSplatSystem: SystemBase
{
    private EntityCommandBufferSystem ecbs;
    protected override void OnCreate()
    {
        ecbs = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
    }

    protected override void OnUpdate()
    {
        var time = Time.ElapsedTime;
        float floorHeight = GetSingleton<SpawnZones>().LevelBounds.Min.y;

        var ecb = ecbs.CreateCommandBuffer().AsParallelWriter();
        Entities
            .WithName("BloodLifeSystem")
            .WithAll<BloodTag>()
            .WithNone<GroundedTime>()
            .ForEach((Entity e, int entityInQueryIndex, ref RandomComponent rng, in PhysicsData data, in Translation translation) =>
            {
                if (translation.Value.y <= floorHeight + 0.001f)
                {
                    ecb.AddComponent(entityInQueryIndex, e, Lifetime.FromTimeRemaining(rng.Value.NextFloat(2f,4f)));

                    ecb.AddComponent(entityInQueryIndex, e, new GroundedTime
                    {
                        Time = time,
                    });
                }
            }).ScheduleParallel();
        


        Entities
            .WithName("BloodFadeSystem")
            .WithAll<BloodTag>()
            .ForEach((ref URPPropertyLifetime shaderPropLifetime, in Lifetime actualLifetime) =>
            {
                shaderPropLifetime.Value = actualLifetime.NormalizedTimeRemaining;
            }).ScheduleParallel();
    }
}