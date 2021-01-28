using Unity.Entities;
using Unity.Rendering;
using Unity.Transforms;
using Unity.Mathematics;
using UnityEngine;

public class BloodSplatSystem: SystemBase
{
    protected override void OnUpdate()
    {
        var time = Time.ElapsedTime;
        float floorHeight = GetSingleton<SpawnZones>().LevelBounds.Min.y;

        var ecb = new EntityCommandBuffer(Unity.Collections.Allocator.Temp);
        Entities
            .WithAll<BloodTag>()
            .WithNone<GroundedTime>()
            .ForEach((Entity e, ref RandomComponent rng, in PhysicsData data, in Translation translation) =>
            {
                if (translation.Value.y <= floorHeight + 0.001f)
                {
                    ecb.AddComponent(e, Lifetime.FromTimeRemaining(rng.Value.NextFloat(0.5f,1.2f)));

                    ecb.AddComponent(e, new GroundedTime
                    {
                        Time = time,
                    });
                }
            }).Run();
        ecb.Playback(EntityManager);
        ecb.Dispose();


        Entities
            .WithAll<BloodTag>()
            .ForEach((ref URPPropertyLifetime shaderPropLifetime, in Lifetime actualLifetime) =>
            {
                shaderPropLifetime.Value = actualLifetime.NormalizedTimeRemaining;
            }).Run();
    }
}