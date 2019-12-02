using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

namespace AntPheromones_ECS
{
    [UpdateAfter(typeof(RandomizeFacingAngleSystem))]
    public class CalculateSteeringSystem : JobComponentSystem
    {
        private EntityQuery _mapQuery;
        private EntityQuery _pheromoneQuery;

        protected override void OnCreate()
        {
            base.OnCreate();
            this._mapQuery = GetEntityQuery(ComponentType.ReadOnly<Map>());
            this._pheromoneQuery = GetEntityQuery(ComponentType.ReadOnly<PheromoneColourRValueBuffer>());
        }

        protected override JobHandle OnUpdate(JobHandle inputDependencies)
        {
            inputDependencies.Complete(); 
            
            Entity pheromoneRValues = this._pheromoneQuery.GetSingletonEntity();
            DynamicBuffer<PheromoneColourRValueBuffer> pheromoneColourRValues = 
                GetBufferFromEntity<PheromoneColourRValueBuffer>(isReadOnly: true)[pheromoneRValues];
            
            Map map = this._mapQuery.GetSingleton<Map>();
            
            JobHandle pheromoneSteeringJob = new CalculatePheromoneSteeringJob
            {
                MapWidth = map.Width,
                PheromoneRValues = pheromoneColourRValues
            }.ScheduleSingle(this, inputDependencies);

            JobHandle wallSteeringJob = new CalculateWallSteeringJob
            {
                Obstacles = map.Obstacles,
                MapWidth = map.Width
            }.Schedule(this, inputDependencies);
            
            return JobHandle.CombineDependencies(wallSteeringJob, pheromoneSteeringJob);
        }

        [BurstCompile]
        private struct CalculatePheromoneSteeringJob : IJobForEach<Position, FacingAngle, PheromoneSteering>
        {
            public DynamicBuffer<PheromoneColourRValueBuffer> PheromoneRValues;
            public int MapWidth;

            private const float Distance = 3;
            
            public void Execute(
                [ReadOnly] ref Position position,
                [ReadOnly] ref FacingAngle facingAngle,
                [WriteOnly] ref PheromoneSteering steering)
            {
                float result = 0;

                for (int i = -1; i <= 1; i += 2) 
                {
                    float angle = facingAngle.Value + i * Mathf.PI * 0.25f;
                    
                    math.sincos(angle, out float sin, out float cos);
                    int2 targetDestination = (int2) (position.Value + Distance * new float2(cos, sin));
                    
                    if (math.any(targetDestination < 0) || math.any(targetDestination >= MapWidth))
                    {
                        continue;
                    }
                    result += this.PheromoneRValues[targetDestination.x + targetDestination.y * this.MapWidth] * i;
                }

                steering.Value = result >= 0 ? 1 : -1;
            }
        }
        
        [BurstCompile]
        private struct CalculateWallSteeringJob : IJobForEach<Position, FacingAngle, WallSteering>
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

                    if (this.Obstacles.Value.TryGetObstacles(targetPosition).Exist)
                    {
                        result -= i;
                    }
                }
                steering.Value = result;
            }
        }
    }
}