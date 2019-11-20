using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;

namespace AntPheromones_ECS
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateAfter(typeof(CollideWithObstacleSystem))]
    public class MoveRadiallySystem : JobComponentSystem
    {
        private MapComponent _map;
        private SteeringStrengthComponent _steeringStrength;

        protected override void OnCreate()
        {
            base.OnCreate();
            
            this._map = GetEntityQuery(ComponentType.ReadOnly<MapComponent>()).GetSingleton<MapComponent>();
            this._steeringStrength = 
                GetEntityQuery(ComponentType.ReadOnly<SteeringStrengthComponent>()).GetSingleton<SteeringStrengthComponent>();
        }

        protected override JobHandle OnUpdate(JobHandle inputDependencies)
        {
            var carryJob = new CarryJob
            {
                Strength = this._steeringStrength.Inward,
                ColonyPosition = this._map.ColonyPosition,
                PushRadius = this._map.Width
            }.Schedule(this, inputDependencies);
            
            var searchJob = new SearchJob
            {
                Strength = -this._steeringStrength.Outward,
                PushRadius = this._map.Width * 0.4f,
                ColonyPosition = this._map.ColonyPosition
            }.Schedule(this, inputDependencies);
            
            return JobHandle.CombineDependencies(carryJob, searchJob);
        }
        
        private struct SearchJob : IJobForEach<PositionComponent, VelocityComponent>
        {
            public float2 ColonyPosition;
            public float Strength;
            public float PushRadius;
            
            public void Execute([ReadOnly] ref PositionComponent position, ref VelocityComponent velocity)
            {
                float2 offset = this.ColonyPosition - position.Value;
                float distance = math.length(offset);
                velocity.Value += offset / distance * Strength * (1f - math.clamp(distance / PushRadius, 0f, 1f));
            }
        }
        
        private struct CarryJob : IJobForEach<PositionComponent, VelocityComponent>
        {
            public float2 ColonyPosition;
            public float Strength;
            public float PushRadius;
            
            public void Execute([ReadOnly] ref PositionComponent position, ref VelocityComponent velocity)
            {
                float2 offset = this.ColonyPosition - position.Value;
                float distance = math.length(offset);
                velocity.Value += offset / distance * Strength * (1f - math.clamp(distance / PushRadius, 0f, 1f));
            }
        }
    }
}