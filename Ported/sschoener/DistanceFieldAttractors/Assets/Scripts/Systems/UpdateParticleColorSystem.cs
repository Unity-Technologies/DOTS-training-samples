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
            var surfaceColor = particleSetup.SurfaceColor;
            var exteriorColor = particleSetup.ExteriorColor;
            var interiorColor = particleSetup.InteriorColor;
            var colorStiffness = particleSetup.ColorStiffness;
            var exteriorColorDist = particleSetup.ExteriorColorDist;
            var interiorColorDist = particleSetup.InteriorColorDist;
            return Entities.ForEach((ref RenderColorComponent color, in PositionInDistanceFieldComponent fieldPosition) =>
            {
                
                Color otherColor = fieldPosition.Distance > 0 ? exteriorColor : interiorColor;
                float distance = fieldPosition.Distance > 0 ? exteriorColorDist : -interiorColorDist;
                Color targetColor = Color.Lerp(surfaceColor, otherColor, fieldPosition.Distance / distance);
                color.Value = Color.Lerp(color.Value, targetColor, (1 - colorStiffness) / 60f);
            }).Schedule(inputDeps);
        }
    }
}
