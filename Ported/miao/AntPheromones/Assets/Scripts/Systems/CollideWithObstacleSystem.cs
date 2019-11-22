using Unity.Burst;
using Unity.Collections;

namespace AntPheromones_ECS
{
    using Unity.Entities;
    using Unity.Jobs;
    using Unity.Mathematics;

    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateAfter(typeof(ChangePositionAndVelocityAfterCarryingResourceSystem))]
    public class CollideWithObstacleSystem : JobComponentSystem
    {
        private EntityQuery _mapQuery;

        protected override void OnCreate()
        {
            base.OnCreate();
            this._mapQuery = GetEntityQuery(ComponentType.ReadOnly<MapComponent>());
        }

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            var map = this._mapQuery.GetSingleton<MapComponent>();
            
            return new Job 
            {
                ObstacleRadius = map.Obstacles.Value.Radius,
                Obstacles = map.Obstacles
            }.Schedule(this, inputDeps);
        }

        [BurstCompile]
        private struct Job : IJobForEach<PositionComponent, VelocityComponent>
        {
            public float ObstacleRadius;
            public BlobAssetReference<Obstacles> Obstacles;
            
            public void Execute(ref PositionComponent position, ref VelocityComponent velocity)
            {
                for (int obstacle = 0; obstacle < Obstacles.Value.Positions.Length; obstacle++)
                {
                    float2 obstaclePosition = Obstacles.Value.Positions[obstacle];
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