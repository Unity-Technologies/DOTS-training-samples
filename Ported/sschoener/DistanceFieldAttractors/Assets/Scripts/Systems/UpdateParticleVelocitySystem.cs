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
            var attraction = particleSetup.Attraction;
            var jitter = particleSetup.Jitter;
            var seed = 1 + (uint)UnityEngine.Time.frameCount;
            return Entities.ForEach((Entity entity, ref PositionComponent position, ref VelocityComponent velocity, in PositionInDistanceFieldComponent fieldPosition) =>
            {
                var rng = new Random(seed * (uint)(1 + entity.Index));
                float3 deltaV = -fieldPosition.Normal * attraction * math.clamp(fieldPosition.Distance, -1, 1);
                deltaV += rng.NextFloat3Direction() * rng.NextFloat() * jitter;
                velocity.Value = .99f * (velocity.Value + deltaV);
                position.Value += velocity.Value;
            }).WithBurst(FloatMode.Fast).Schedule(inputDeps);
        }
    }
}
