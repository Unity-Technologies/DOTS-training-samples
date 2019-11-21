using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;

namespace AntPheromones_ECS
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    public class OrientTowardsGoalSystem : JobComponentSystem
    {
        private MapComponent _map;
        private SteeringStrengthComponent _steeringStrengthComponent;

        protected override void OnCreate()
        {
            base.OnCreate();
            
            this._map =
                GetEntityQuery(ComponentType.ReadOnly<MapComponent>()).GetSingleton<MapComponent>();
            this._steeringStrengthComponent =
                GetEntityQuery(ComponentType.ReadOnly<SteeringStrengthComponent>()).GetSingleton<SteeringStrengthComponent>();
        }

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            return new Job {
                GoalSteerStrength =  this._steeringStrengthComponent.Goal,
                ColonyPosition = this._map.ColonyPosition,
                ResourcePosition = this._map.ResourcePosition,
                Obstacles = this._map.Obstacles.Value
            }.Schedule(this, inputDeps);
        }

        [BurstCompile]
        struct Job : IJobForEach<PositionComponent, ResourceCarrierComponent, FacingAngleComponent>
        {
            public float2 ColonyPosition;
            public float2 ResourcePosition;
            public float GoalSteerStrength;
            
            public ObstacleData Obstacles;
            
            public void Execute([ReadOnly] ref PositionComponent position, [ReadOnly] ref ResourceCarrierComponent resourcCarrier, ref FacingAngleComponent facingAngle)
            {
                float2 targetPosition = resourcCarrier.IsCarrying ? this.ColonyPosition : this.ResourcePosition;

                if (HasObstaclesBetween(position.Value, targetPosition))
                {
                    return;
                }
                
                float targetAngle = math.atan2(targetPosition.y - position.Value.y, targetPosition.x - position.Value.x);
                float offset = targetAngle - facingAngle.Value;
                    
                if (offset > math.PI)
                {
                    facingAngle.Value += math.PI * 2f;
                }
                else if (offset < -math.PI)
                {
                    facingAngle.Value -= math.PI * 2f;
                }
                else if (math.abs(offset) < math.PI * 0.5f)
                {
                    facingAngle.Value += offset * this.GoalSteerStrength;
                }
            }
            
            private bool HasObstaclesBetween(float2 point1, float2 point2)
            {
                float2 offset = point2 - point1;
                float distance = math.lengthsq(offset);

                int stepCount = (int)math.ceil(distance * 0.5f);
                for (int i = 0; i < stepCount; i++)
                {
                    float t = (float)i / stepCount;
                        
                    if (this.Obstacles.HasObstacle(point1 + t * offset))
                    {
                        return true;
                    }
                }

                return false;
            }
        }
    }
}