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
                for (int obstacle = 0; obstacle < Obstacles.Value.Positions.Length; obstacle++)
                {
                    float2 obstaclePosition = Obstacles.Value.Positions[obstacle];
                    float2 offset = position.Value - obstaclePosition;
                    
                    float distanceSquared = math.lengthsq(offset);

                    if (distanceSquared >= ObstacleRadius * ObstacleRadius)
                    {
                        continue;
                    }
                    
                    offset /= math.sqrt(distanceSquared);
                    
                    position.Value = obstaclePosition + offset * ObstacleRadius;
                    velocity.Value -= 1.5f * offset * math.dot(offset, velocity.Value);
                }
            }
        }
    }
}