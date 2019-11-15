using System;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;

namespace Systems {
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
            return new UpdateLocalToWorld
            {
                SpeedStretch = particleSetup.SpeedStretch,
            }.Schedule(this, inputDeps);
        }
        
        
        [BurstCompile]
        struct UpdateLocalToWorld : IJobForEach<PositionComponent, VelocityComponent, LocalToWorldComponent>
        {
            public float SpeedStretch;

            public void Execute(
                [ReadOnly] ref PositionComponent position,
                [ReadOnly] ref VelocityComponent velocity,
                ref LocalToWorldComponent localToWorld)
            {
                float3 up = new float3(0, 0, 1);
                quaternion rotation = quaternion.LookRotation(math.normalize(velocity.Value), up);
                float3 scale = new float3(.1f, .01f, math.max(
                    .1f, math.length(velocity.Value) * SpeedStretch
                ));
                localToWorld.Value = float4x4.TRS(position.Value, rotation, scale);
            }
        }
    }
}
