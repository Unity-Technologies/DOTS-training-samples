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
        
        
        [BurstCompile(FloatMode = FloatMode.Fast)]
        struct UpdateLocalToWorld : IJobForEach<PositionComponent, VelocityComponent, LocalToWorldComponent>
        {
            public float SpeedStretch;

            public void Execute(
                [ReadOnly] ref PositionComponent position,
                [ReadOnly] ref VelocityComponent velocity,
                ref LocalToWorldComponent localToWorld)
            {
                float speed = math.length(velocity.Value);
                
                quaternion rotation = quaternion.LookRotation(velocity.Value/speed, new float3(0, 0, 1));
                
                float3 scale = new float3(.1f, .01f, math.max(.1f, speed * SpeedStretch));
                
                localToWorld.Value = float4x4.TRS(position.Value, rotation, scale);
            }
        }
    }
}
