using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;

[UpdateInGroup(typeof(SimulationSystemGroup))]
[UpdateAfter(typeof(ObstacleCollisionSystem))]
public class RadialMovementSystem : JobComponentSystem
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
        var inwardStrength = antSteering.InwardSteerStrength;
        var inwardPushRadius = map.MapSize;
        var outwardStrength = -antSteering.OutwardSteerStrength;
        var outwardPushRadius = map.MapSize * .4f;
        return Entities.ForEach((ref VelocityComponent velocity, in PositionComponent position, in HasResourcesComponent hasResources) =>
        {
            var strength = hasResources.Value ? inwardStrength : outwardStrength;
            var pushRadius = hasResources.Value ? inwardPushRadius : outwardPushRadius;
            var delta = colonyPosition - position.Value;
            float dist = math.length(delta);

            strength *= 1f - math.clamp(dist / pushRadius, 0f, 1f);
            velocity.Value += delta * (strength / dist);
        }).Schedule(inputDeps);
    }
}
