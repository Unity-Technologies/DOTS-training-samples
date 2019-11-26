using System;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;

[UpdateInGroup(typeof(SimulationSystemGroup))]
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
        var obstacleRef = map.Obstacles;

        return Entities.ForEach((ref FacingAngleComponent facingAngle, in PositionComponent position, in HasResourcesComponent hasResources) =>
        {
            var targetPos = hasResources.Value ? colonyPosition : resourcePosition;
            var p = position.Value;
            ref var obstacles = ref obstacleRef.Value;
            if (!Linecast(p, targetPos, ref obstacles))
            {
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
    
    static bool Linecast(float2 point1, float2 point2, ref ObstacleData obstacles)
    {
        float2 d = point2 - point1;
        float dist = math.length(d);

        int stepCount = (int)math.ceil(dist * .5f);
        for (int i = 0; i < stepCount; i++)
        {
            float t = (float)i / stepCount;
            if (obstacles.HasObstacle(point1 + t * d))
                return true;
        }
        return false;
    }
}
