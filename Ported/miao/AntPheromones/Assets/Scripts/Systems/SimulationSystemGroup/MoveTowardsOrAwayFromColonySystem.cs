using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;

namespace AntPheromones_ECS
{
    [UpdateAfter(typeof(AvoidObstacleSystem))]
    public class MoveTowardsOrAwayFromColonySystem : JobComponentSystem
    {
        private EntityQuery _mapQuery;
        private EntityQuery _steeringStrengthQuery;
        private (bool AreRetrieved, float Inward, float Outward) _steeringStrengths;

        protected override void OnCreate()
        {
            base.OnCreate();
            
            this._mapQuery = GetEntityQuery(ComponentType.ReadOnly<Map>());
            this._steeringStrengthQuery = 
                GetEntityQuery(ComponentType.ReadOnly<SteeringStrength>());
        }

        protected override JobHandle OnUpdate(JobHandle inputDependencies)
        {
            if (!this._steeringStrengths.AreRetrieved)
            {
                var steeringStrength = this._steeringStrengthQuery.GetSingleton<SteeringStrength>();
                this._steeringStrengths =
                    (AreRetrieved: true,
                    Inward: steeringStrength.Inward,
                    Outward: steeringStrength.Outward);
            }
            
            var map = this._mapQuery.GetSingleton<Map>();
            
            return new Job
            {
                ColonyPosition = map.ColonyPosition,
                
                InwardStrength = this._steeringStrengths.Inward,
                InwardPushRadius = map.Width,
                
                OutwardStrength = -this._steeringStrengths.Outward,
                OutwardPushRadius = map.Width * 0.4f
            }.Schedule(this, inputDependencies);
        }

        [BurstCompile]
        private struct Job : IJobForEach<Position, ResourceCarrier, Velocity, FacingAngle>
        {
            public float2 ColonyPosition;

            public float InwardStrength;
            public float InwardPushRadius;
            
            public float OutwardStrength;
            public float OutwardPushRadius;
            
            public void Execute(
                [ReadOnly]ref Position position, 
                [ReadOnly] ref ResourceCarrier resourceCarrier,
                ref Velocity velocity,
                [WriteOnly] ref FacingAngle facingAngle)
            {
                float strength = resourceCarrier.IsCarrying ? this.InwardStrength : this.OutwardStrength;

                float2 offset = this.ColonyPosition - position.Value;
                float distance = math.length(offset);
                
                strength *=
                    1f - math.clamp(distance / (resourceCarrier.IsCarrying ? this.InwardPushRadius : this.OutwardPushRadius), 0f, 1f);
                velocity.Value += offset * (strength / distance);
                facingAngle.Value = math.atan2(velocity.Value.y, velocity.Value.x);
            }
        }
    }
}