using Unity.Burst;
using Unity.Collections;
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
        return new Job {
            ColonyPosition = map.ColonyPosition,
            InwardStrength = antSteering.InwardSteerStrength,
            InwardPushRadius = map.MapSize,
            OutwardStrength = -antSteering.OutwardSteerStrength,
            OutwardPushRadius = map.MapSize * .4f
        }.Schedule(this, inputDeps);
    }

    [BurstCompile]
    struct Job  : IJobForEach<PositionComponent, HasResourcesComponent, VelocityComponent>
    {
        public float2 ColonyPosition;
        public float InwardStrength;
        public float OutwardStrength;
        public float InwardPushRadius;
        public float OutwardPushRadius;
        
        public void Execute([ReadOnly] ref PositionComponent position, [ReadOnly] ref HasResourcesComponent hasResources, [WriteOnly] ref VelocityComponent velocity)
        {
            var strength = hasResources.Value ? InwardStrength : OutwardStrength;
            var pushRadius = hasResources.Value ? InwardPushRadius : OutwardPushRadius;
            var delta = ColonyPosition - position.Value;
            float dist = math.length(delta);

            strength *= 1f - math.clamp(dist / pushRadius, 0f, 1f);
            velocity.Value += delta * (strength / dist);
        }
    }
    
}
