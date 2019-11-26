using System;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

[UpdateInGroup(typeof(SimulationSystemGroup))]
[UpdateAfter(typeof(TargetingSystem))]
public class ResourceCarrySystem : JobComponentSystem
{
    EntityQuery m_MapQuery;

    protected override void OnCreate()
    {
        base.OnCreate();
        m_MapQuery = GetEntityQuery(ComponentType.ReadOnly<MapSettingsComponent>());
    }

    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        var map = m_MapQuery.GetSingleton<MapSettingsComponent>();
        var colonyPosition = map.ColonyPosition;
        var resourcePosition = map.ResourcePosition;
        return Entities.ForEach((ref FacingAngleComponent facingAngle, ref HasResourcesComponent hasResources, in PositionComponent position) =>
        {
            float2 target = math.select(resourcePosition, colonyPosition, hasResources.Value);
            if (math.lengthsq(position.Value - target) < 4f * 4f) {
                facingAngle.Value += math.PI;
                hasResources.Value = !hasResources.Value;
            }
        }).Schedule(inputDeps);
    }
}
