using Unity.Burst;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;

namespace AntPheromones_ECS
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
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
                this.Obstacles.Value.TryGetObstacles(position.Value, out int indexOfCurrentBucket, out int distanceToNextBucket);
                
                for (int i = indexOfCurrentBucket; i < indexOfCurrentBucket + distanceToNextBucket; i++)
                {
                    float2 obstaclePosition = Obstacles.Value.Positions[i];
                    float2 delta = position.Value - obstaclePosition;
                    
                    float distanceSquared = math.lengthsq(delta);

                    if (distanceSquared >= ObstacleRadius * ObstacleRadius)
                    {
                        continue;
                    }
                    
                    delta /= math.sqrt(distanceSquared);
                    
                    position.Value = obstaclePosition + delta * ObstacleRadius;
                    velocity.Value -= 1.5f * delta * math.dot(delta, velocity.Value);
                }
            }
        }
    }
}