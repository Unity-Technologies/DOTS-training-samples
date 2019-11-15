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
                Rng = new Random((1 + (uint)Time.frameCount) * 104729),
                SphereRadius = 50,
            }.Schedule(this, inputDeps);
        }

        [BurstCompile]
        [RequireComponentTag(typeof(UninitializedTagComponent))]
        struct InitializePositionsJob : IJobForEach<PositionComponent>
        {
            public Random Rng;
            public float SphereRadius;
            public void Execute(ref PositionComponent position)
            {
                position.Value = SphereRadius * Rng.NextFloat3Direction() * Rng.NextFloat();
            }
        }
    }
}
