using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;

namespace AntPheromones_ECS
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateAfter(typeof(ChangeSpeedAccordingToSteeringStrengthSystem))]
    public class OrientTowardsGoalSystem : JobComponentSystem
    {
        private EntityQuery _mapQuery;
        private EntityQuery _steeringStrengthQuery;
        private (bool IsRetrieved, float Goal) _steeringStrength;

        protected override void OnCreate()
        {
            base.OnCreate();
            
            this._mapQuery =
                GetEntityQuery(ComponentType.ReadOnly<MapComponent>());
            this._steeringStrengthQuery =
                GetEntityQuery(ComponentType.ReadOnly<SteeringStrengthComponent>());
        }

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            if (!this._steeringStrength.IsRetrieved)
            {
                var steeringStrength = this._steeringStrengthQuery.GetSingleton<SteeringStrengthComponent>();
                this._steeringStrength = (IsRetrieved: true, steeringStrength.Goal);
            }

            var map = this._mapQuery.GetSingleton<MapComponent>();
            
            return new Job
            {
                GoalSteerStrength =  this._steeringStrength.Goal,
                
                ColonyPosition = map.ColonyPosition,
                ResourcePosition = map.ResourcePosition,
                Obstacles = map.Obstacles
            }.Schedule(this, inputDeps);
        }

        [BurstCompile]
        struct Job : IJobForEach<PositionComponent, ResourceCarrierComponent, FacingAngleComponent>
        {
            public float2 ColonyPosition;
            public float2 ResourcePosition;
            public float GoalSteerStrength;
            
            public BlobAssetReference<Obstacles> Obstacles;
            
            public void Execute(
                [ReadOnly] ref PositionComponent position, 
                [ReadOnly] ref ResourceCarrierComponent resourcCarrier, 
                ref FacingAngleComponent facingAngle)
            {
                float2 targetPosition = resourcCarrier.IsCarrying ? this.ColonyPosition : this.ResourcePosition;

                if (HasObstaclesBetween(position.Value, targetPosition))
                {
                    return;
                }
                
                float targetAngle = 
                    math.atan2(targetPosition.y - position.Value.y, targetPosition.x - position.Value.x);
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

                    var position = point1 + t * offset;
                    if (this.Obstacles.Value.HasObstacle(position))
                    {
                        return true;
                    }
                }
                return false;
            }
        }
    }
}