namespace AntPheromones_ECS
{
    using Unity.Entities;
    using Unity.Jobs;
    using Unity.Mathematics;

    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateAfter(typeof(ChangePositionAndVelocitySystem))]
    public class CollideWithObstacleSystem : JobComponentSystem
    {
        private MapComponent _map;

        protected override void OnCreate()
        {
            base.OnCreate();
            this._map = GetEntityQuery(ComponentType.ReadOnly<MapComponent>()).GetSingleton<MapComponent>();
        }

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            return new Job {
                ObstacleRadius = this._map.ObstacleRadius,
                Obstacles = this._map.Obstacles.Value.Positions
            }.Schedule(this, inputDeps);
        }

        private struct Job : IJobForEach<PositionComponent, VelocityComponent>
        {
            public float ObstacleRadius;
            public BlobArray<float2> Obstacles;
        
            public void Execute(ref PositionComponent position, ref VelocityComponent velocity)
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