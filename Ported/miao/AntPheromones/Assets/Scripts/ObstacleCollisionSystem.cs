namespace AntPheromones_ECS
{
    using Unity.Entities;
    using Unity.Jobs;
    using Unity.Mathematics;

    [UpdateInGroup(typeof(SimulationSystemGroup))]
//    [UpdateAfter(typeof(UpdatePositionSystem))]
    public class ObstacleCollisionSystem : JobComponentSystem
    {
        private Map _map;

        protected override void OnCreate()
        {
            base.OnCreate();
            this._map = GetEntityQuery(ComponentType.ReadOnly<Map>()).GetSingleton<Map>();
        }

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            return new Job {
                ObstacleRadius = this._map.ObstacleRadius,
                Obstacles = this._map.Obstacles.Value.Obstacles
            }.Schedule(this, inputDeps);
        }

        private struct Job : IJobForEach<Position, Velocity>
        {
            public float ObstacleRadius;
            public BlobArray<float2> Obstacles;
        
            public void Execute(ref Position position, ref Velocity velocity)
            {
                for (int obstacle = 0; obstacle < Obstacles.Length; obstacle++)
                {
                    float2 obstaclePosition = Obstacles[obstacle];
                    float2 offset = position.Value - obstaclePosition;
                    
                    float distanceSquared = math.lengthsq(offset);

                    if (!(distanceSquared < ObstacleRadius * ObstacleRadius))
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