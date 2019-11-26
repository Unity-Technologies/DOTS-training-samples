using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;

namespace AntPheromones_ECS
{
    [UpdateAfter(typeof(RandomizeFacingAngleSystem))]
    public class CalculateWallSteeringSystem : JobComponentSystem
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
                Obstacles = map.Obstacles,
                MapWidth = map.Width
            }.Schedule(this, inputDeps);
        }

        [BurstCompile]
        private struct Job : IJobForEach<Position, FacingAngle, WallSteering>
        {
            public float MapWidth;
            public BlobAssetReference<Obstacles> Obstacles;
            
            private const float Distance = 1.5f;
            
            public void Execute(
                [ReadOnly] ref Position position, 
                [ReadOnly] ref FacingAngle facingAngle,
                ref WallSteering steering)
            {
                float result = 0;

                for (int i = -1; i <= 1; i += 2) 
                {
                    float angle = facingAngle.Value + i * math.PI * 0.25f;
                    
                    math.sincos(angle, out float sin, out float cos);
                    float2 targetPosition = position.Value + Distance * new float2(cos, sin);

                    bool targetDestinationOutOfMapBounds =
                        math.any(targetPosition < 0) || math.any(targetPosition >= this.MapWidth);
                    
                    if (targetDestinationOutOfMapBounds)
                    {
                        continue;
                    }

                    if (this.Obstacles.Value.HasObstacle(targetPosition))
                    {
                        result -= i;
                    }
                }
                steering.Value = result;
            }
        }
    }
}