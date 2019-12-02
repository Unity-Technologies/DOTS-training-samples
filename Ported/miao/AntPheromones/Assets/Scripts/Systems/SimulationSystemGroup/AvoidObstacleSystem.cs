using Unity.Burst;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;

namespace AntPheromones_ECS
{
    [UpdateAfter(typeof(UpdatePositionAndVelocityAfterMovingResourceSystem))]
    public class AvoidObstacleSystem : JobComponentSystem
    {
        private EntityQuery _mapQuery;

        protected override void OnCreate()
        {
            base.OnCreate();
            this._mapQuery = GetEntityQuery(ComponentType.ReadOnly<Map>());
        }

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            var map = this._mapQuery.GetSingleton<Map>();
            
            return new Job 
            {
                ObstacleRadius = map.Obstacles.Value.Radius,
                Obstacles = map.Obstacles
            }.Schedule(this, inputDeps);
        }

        [BurstCompile]
        private struct Job : IJobForEach<Position, Velocity>
        {
            public float ObstacleRadius;
            public BlobAssetReference<Obstacles> Obstacles;
            
            public void Execute(ref Position position, ref Velocity velocity)
            {
                var obstacles = this.Obstacles.Value.TryGetObstacles(position.Value);

                if (!obstacles.Exist)
                {
                    return;
                }
                
                for (int i = obstacles.IndexOfCurrentBucket.Value;
                    i < obstacles.IndexOfCurrentBucket.Value + obstacles.DistanceToNextBucket.Value;
                    i++)
                {
                    float2 obstaclePosition = this.Obstacles.Value.Positions[i];
                    float2 distanceToObstaclePosition = position.Value - obstaclePosition;
                    
                    float distanceSquared = math.lengthsq(distanceToObstaclePosition);

                    bool obstacleIsTooFarAway = distanceSquared >= this.ObstacleRadius * this.ObstacleRadius;
                    
                    if (obstacleIsTooFarAway)
                    {
                        continue;
                    }
                    
                    distanceToObstaclePosition /= math.sqrt(distanceSquared);
                    
                    position.Value = obstaclePosition + distanceToObstaclePosition * this.ObstacleRadius;
                    velocity.Value -= 1.5f * distanceToObstaclePosition * math.dot(distanceToObstaclePosition, velocity.Value);
                }
            }
        }
    }
}