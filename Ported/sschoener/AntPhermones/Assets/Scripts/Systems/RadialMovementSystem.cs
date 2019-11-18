using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;

[UpdateInGroup(typeof(SimulationSystemGroup))]
[UpdateAfter(typeof(ObstacleCollisionSystem))]
public class RadialMovementSystem : JobComponentSystem
{
    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        throw new System.NotImplementedException();
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
