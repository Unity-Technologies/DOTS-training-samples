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

        Entities
            .WithName("BloodLifeSystem")
            .WithAll<BloodTag>()
            .ForEach((ref Lifetime lifetime, ref RandomComponent rng, in PhysicsData data, in Translation translation) =>
            {
                if (translation.Value.y <= floorHeight + 0.001f && lifetime.NormalizedDecaySpeed == 0)
                {
                    lifetime.NormalizedDecaySpeed = 1 / rng.Value.NextFloat(2f, 4f);
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