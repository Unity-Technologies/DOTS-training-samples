using System;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Random = Unity.Mathematics.Random;

namespace Systems {
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateAfter(typeof(UpdateParticleDistanceSystem))]
    public class UpdateParticleVelocitySystem : JobComponentSystem
    {
        EntityQuery m_ParticleSetup;

        protected override void OnCreate()
        {
            base.OnCreate();
            m_ParticleSetup = GetEntityQuery(
                ComponentType.ReadOnly<ParticleSetupComponent>()
            );
        }

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            var particleSetup = EntityManager.GetSharedComponentData<ParticleSetupComponent>(m_ParticleSetup.GetSingletonEntity());
            return new UpdateVelocity
            {
                Attraction = particleSetup.Attraction,
                Jitter = particleSetup.Jitter,
                Rng = new Random((uint)UnityEngine.Time.frameCount)
            }.Schedule(this, inputDeps);
        }

        [BurstCompile(FloatMode = FloatMode.Fast)]
        struct UpdateVelocity : IJobForEach<PositionInDistanceFieldComponent, PositionComponent, VelocityComponent>
        {
            public Random Rng;
            public float Attraction;
            public float Jitter;
            
            public void Execute(
                [ReadOnly] ref PositionInDistanceFieldComponent fieldPosition,
                ref PositionComponent position,
                ref VelocityComponent velocity)
            {
                float3 deltaV = -fieldPosition.Normal * Attraction * math.clamp(fieldPosition.Distance, -1, 1);
                deltaV += Rng.NextFloat3Direction() * Rng.NextFloat() * Jitter;
                velocity.Value = .99f * (velocity.Value + deltaV);
                position.Value += velocity.Value;
            }
        }
    }
}
