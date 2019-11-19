using System;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using UnityEngine;

[UpdateInGroup(typeof(PresentationSystemGroup))]
public class UpdateAntColorSystem : JobComponentSystem
{
    EntityQuery m_AntSettingsQuery;

    protected override void OnCreate()
    {
        base.OnCreate();
        m_AntSettingsQuery = GetEntityQuery(ComponentType.ReadOnly<AntRenderSettingsComponent>());
    }

    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        var ants = m_AntSettingsQuery.GetSingleton<AntRenderSettingsComponent>();
        return new Job
        {
            SearcherColor = ants.SearcherColor,
            CarrierColor = ants.CarrierColor
        }.Schedule(this, inputDeps);
    }

    [BurstCompile]
    struct Job : IJobForEach<BrightnessComponent, HasResourcesComponent, RenderColorComponent>
    {
        public Color SearcherColor;
        public Color CarrierColor;

        public void Execute(
            [ReadOnly] ref BrightnessComponent brightness,
            [ReadOnly] ref HasResourcesComponent hasResources,
            [WriteOnly] ref RenderColorComponent renderColor)
        {
            var c = hasResources.Value ? CarrierColor : SearcherColor;
            renderColor.Value += (c * brightness.Value - renderColor.Value) * .05f;
        }
    }
}
