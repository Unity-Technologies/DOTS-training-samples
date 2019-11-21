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
            return new Job
            {
                ColonyPosition = this._map.ColonyPosition,
                
                InwardStrength = this._steeringStrength.Inward,
                InwardPushRadius = this._map.Width,
                
                OutwardStrength = this._steeringStrength.Outward,
                OutwardPushRadius = this._map.Width * 0.4f
            }.Schedule(this, inputDependencies);
        }

        private struct Job : IJobForEach<PositionComponent, VelocityComponent, ResourceCarrierComponent>
        {
            public float2 ColonyPosition;

            public float InwardStrength;
            public float InwardPushRadius;
            
            public float OutwardStrength;
            public float OutwardPushRadius;
            
            public void Execute(ref PositionComponent position, ref VelocityComponent velocity, ref ResourceCarrierComponent resourceCarrier)
            {
                float pushRadius = resourceCarrier.IsCarrying ? this.InwardPushRadius : this.OutwardPushRadius;
                float strength = resourceCarrier.IsCarrying ? this.InwardStrength : this.OutwardStrength;

                float2 offset = this.ColonyPosition - position.Value;
                float distance = math.length(offset);
                
                velocity.Value += offset / distance * strength * (1f - math.clamp(distance / pushRadius, 0f, 1f));
            }
        }
    }
}