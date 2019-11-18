using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;

[UpdateInGroup(typeof(SimulationSystemGroup))]
[UpdateAfter(typeof(ObstacleCollisionSystem))]
public class RadialMovementSystem : JobComponentSystem
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
        var carrier = new CarrierJob {
            Data = {
                ColonyPosition = map.ColonyPosition,
                Strength = map.InwardStrength,
                PushRadius = map.MapSize
            }
        }.Schedule(this, inputDeps);
        var searcher = new SearcherJob {
            Data = {
                ColonyPosition = map.ColonyPosition,
                Strength = -map.OutwardStrength,
                PushRadius = map.MapSize * .4f
            }
        }.Schedule(this, inputDeps);
        return JobHandle.CombineDependencies(carrier, searcher);
    }

    struct BaseJob
    {
        public float2 ColonyPosition;
        public float Strength;
        public float PushRadius;
        
        public void Execute([ReadOnly] ref PositionComponent position, [WriteOnly] ref VelocityComponent velocity)
        {
            var delta = ColonyPosition - position.Value;
            float dist = math.length(delta);
            velocity.Value += delta / dist * Strength * (1f - math.clamp(dist / PushRadius, 0f, 1f));
        }
    }
    
    [RequireComponentTag(typeof(HasResourcesTagComponent))]
    struct CarrierJob : IJobForEach<PositionComponent, VelocityComponent>
    {
        public BaseJob Data;
        public void Execute([ReadOnly] ref PositionComponent position, [WriteOnly] ref VelocityComponent velocity)
        {
            Data.Execute(ref position, ref velocity);
        }
    }

    [ExcludeComponent((typeof(HasResourcesTagComponent)))]
    struct SearcherJob : IJobForEach<PositionComponent, VelocityComponent>
    {
        public BaseJob Data;
        public void Execute([ReadOnly] ref PositionComponent position, [WriteOnly] ref VelocityComponent velocity)
        {
            Data.Execute(ref position, ref velocity);
        }
    }
}
