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
            return new InitializePositionsJob
            {
                Seed = (1 + (uint)Time.frameCount) * 104729,
                SphereRadius = 50,
            }.Schedule(this, inputDeps);
        }

        [BurstCompile]
        [RequireComponentTag(typeof(UninitializedTagComponent))]
        struct InitializePositionsJob : IJobForEachWithEntity<PositionComponent>
        {
            public uint Seed;
            public float SphereRadius;

            public void Execute(Entity entity, int index, ref PositionComponent position)
            {
                var rng = new Random(Seed * (uint)index);
                position.Value = SphereRadius * rng.NextFloat3Direction() * rng.NextFloat();
            }
        }
    }
}
