using System;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using UnityEngine;

namespace Systems {
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateAfter(typeof(UpdateParticleDistanceSystem))]
    public class UpdateParticleColorSystem : JobComponentSystem
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
            return new UpdateColor
            {
                SurfaceColor = particleSetup.SurfaceColor,
                ExteriorColor = particleSetup.ExteriorColor,
                InteriorColor = particleSetup.InteriorColor,
                ColorStiffness = particleSetup.ColorStiffness,
                ExteriorColorDist = particleSetup.ExteriorColorDist,
                InteriorColorDist = particleSetup.InteriorColorDist
            }.Schedule(this, inputDeps);
        }
        
        [BurstCompile]
        struct UpdateColor : IJobForEach<PositionInDistanceFieldComponent, RenderColorComponent>
        {
            public Color SurfaceColor;
            public Color ExteriorColor;
            public Color InteriorColor;
            public float ColorStiffness;
            public float ExteriorColorDist;
            public float InteriorColorDist;
            
            public void Execute(
                [ReadOnly] ref PositionInDistanceFieldComponent fieldPosition,
                ref RenderColorComponent color)
            {
                
                Color targetColor;
                if (fieldPosition.Distance > 0)
                {
                    targetColor = Color.Lerp(SurfaceColor, ExteriorColor, fieldPosition.Distance / ExteriorColorDist);
                }
                else
                {
                    targetColor = Color.Lerp(SurfaceColor, InteriorColor, -fieldPosition.Distance / InteriorColorDist);
                }

                color.Value = Color.Lerp(color.Value, targetColor, (1 - ColorStiffness) / 60f);
            }
        }
    }
}
