using System;
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
        var searcherColor = ants.SearcherColor;
        var carrierColor = ants.CarrierColor;
        return Entities.ForEach((ref RenderColorComponent renderColor, in BrightnessComponent brightness, in HasResourcesComponent hasResources) =>
        {
            var c = hasResources.Value ? carrierColor : searcherColor;
            renderColor.Value += (c * brightness.Value - renderColor.Value) * .05f;
        }).Schedule(inputDeps);
    }
}
