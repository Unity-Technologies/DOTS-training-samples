using Unity.Burst;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;

namespace AntPheromones_ECS
{
    [UpdateAfter(typeof(CalculatePositionAfterTransportingResourceSystem))]
    public class CollideWithObstacleSystem : JobComponentSystem
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
                
                for (int i = obstacles.IndexOfCurrentBucket.Value; i < obstacles.IndexOfCurrentBucket.Value + obstacles.DistanceToNextBucket.Value; i++)
                {
                    float2 obstaclePosition = this.Obstacles.Value.Positions[i];
                    float2 delta = position.Value - obstaclePosition;
                    
                    float distanceSquared = math.lengthsq(delta);

                    if (distanceSquared >= this.ObstacleRadius * this.ObstacleRadius)
                    {
                        continue;
                    }
                    
                    delta /= math.sqrt(distanceSquared);
                    
                    position.Value = obstaclePosition + delta * this.ObstacleRadius;
                    velocity.Value -= 1.5f * delta * math.dot(delta, velocity.Value);
                }
            }
        }
    }
}