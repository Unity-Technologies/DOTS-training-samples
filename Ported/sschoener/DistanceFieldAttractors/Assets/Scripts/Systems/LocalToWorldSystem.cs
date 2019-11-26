using System;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;

namespace Systems
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateAfter(typeof(UpdateParticleVelocitySystem))]
    public class LocalToWorldSystem : JobComponentSystem
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
            var speedStretch = particleSetup.SpeedStretch;
            return Entities.ForEach((ref LocalToWorldComponent localToWorld, in PositionComponent position, in VelocityComponent velocity) =>
            {
                float speed = math.length(velocity.Value);
                quaternion rotation = MathHelpers.LookRotationWithUp(velocity.Value / speed);
                float3 scale = new float3(.1f, .01f, math.max(.1f, speed * speedStretch));
                localToWorld.Value = float4x4.TRS(position.Value, rotation, scale);
            }).Schedule(inputDeps);
        }
    }
}
