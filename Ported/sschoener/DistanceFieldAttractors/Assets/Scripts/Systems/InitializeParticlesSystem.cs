using System;
using Unity.Burst;
using Unity.Entities;
using Unity.Jobs;
using UnityEngine;
using Random = Unity.Mathematics.Random;

namespace Systems {
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    [UpdateAfter(typeof(SpawnParticleSystem))]
    [UpdateBefore(typeof(RemoveUninitializedTagSystem))]
    public class InitializeParticlesSystem : JobComponentSystem
    {
        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            var seed = (1 + (uint)UnityEngine.Time.frameCount) * 104729;
            const float sphereRadius = 50;
            return Entities.ForEach((Entity entity, ref PositionComponent position) =>
            {
                var rng = new Random(seed * (uint)(1 + entity.Index));
                position.Value = sphereRadius * rng.NextFloat3Direction() * rng.NextFloat();
            }).Schedule(inputDeps);
        }
    }
}
