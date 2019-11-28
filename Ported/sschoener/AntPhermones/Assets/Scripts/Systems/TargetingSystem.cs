using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

[UpdateInGroup(typeof(SimulationSystemGroup))]
[UpdateAfter(typeof(ApplySteeringSystem))]
public class TargetingSystem : JobComponentSystem
{
    EntityQuery m_MapQuery;
    EntityQuery m_AntSteeringQuery;

    protected override void OnCreate()
    {
        base.OnCreate();
        m_MapQuery = GetEntityQuery(ComponentType.ReadOnly<MapSettingsComponent>());
        m_AntSteeringQuery = GetEntityQuery(ComponentType.ReadOnly<AntSteeringSettingsComponent>());
    }

    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        var map = m_MapQuery.GetSingleton<MapSettingsComponent>();
        var antSteering = m_AntSteeringQuery.GetSingleton<AntSteeringSettingsComponent>();
        var colonyPosition = map.ColonyPosition;
        var resourcePosition = map.ResourcePosition;
        var targetSteerStrength = antSteering.TargetSteerStrength;
        var visibilityRef = map.Visibility;

        return Entities.ForEach((ref FacingAngleComponent facingAngle, in PositionComponent position, in HasResourcesComponent hasResources) =>
        {
            var p = position.Value;
            ref var visibility = ref visibilityRef.Value;
            if (Visible(p, !hasResources.Value, ref visibility))
            {
                var targetPos = hasResources.Value ? colonyPosition : resourcePosition;
                float targetAngle = math.atan2(targetPos.y - p.y, targetPos.x - p.x);
                float deltaAngle = targetAngle - facingAngle.Value;
                if (deltaAngle > math.PI)
                    facingAngle.Value += math.PI * 2f;
                else if (deltaAngle < -math.PI)
                    facingAngle.Value -= math.PI * 2f;
                else if (math.abs(deltaAngle) < math.PI * .5f)
                    facingAngle.Value += deltaAngle * targetSteerStrength;
            }
        }).Schedule(inputDeps);
    }

    static bool Visible(float2 points, bool resource, ref VisibilityData visibility)
    {
        var coords = math.clamp((int2)math.floor(points), 0, visibility.MapSize - 1);
        var index = coords.y * visibility.MapSize + coords.x;
        var major = index / 8;
        var minor = index % 8;
        int mask = 0x1 << minor;
        if (resource)
            return (visibility.Resource[major] & mask) != 0;
        return (visibility.Colony[major] & mask) != 0;
    }
}
