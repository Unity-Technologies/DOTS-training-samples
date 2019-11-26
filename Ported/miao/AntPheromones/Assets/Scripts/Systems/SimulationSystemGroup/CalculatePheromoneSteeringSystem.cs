using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

namespace AntPheromones_ECS
{
    [UpdateAfter(typeof(RandomizeFacingAngleSystem))]
    public class CalculatePheromoneSteeringSystem : JobComponentSystem
    {
        private EntityQuery _mapQuery;

        protected override void OnCreate()
        {
            base.OnCreate();
            this._mapQuery = GetEntityQuery(ComponentType.ReadOnly<Map>());
        }

        protected override JobHandle OnUpdate(JobHandle inputDependencies)
        {
            inputDependencies.Complete();
            
            Entity pheromoneRValues = 
                GetEntityQuery(ComponentType.ReadOnly<PheromoneColourRValueBuffer>()).GetSingletonEntity();
            DynamicBuffer<PheromoneColourRValueBuffer> pheromoneColourRValues = 
                GetBufferFromEntity<PheromoneColourRValueBuffer>(isReadOnly: true)[pheromoneRValues];
            
            Map map = this._mapQuery.GetSingleton<Map>();
            return new Job
            {
                MapWidth = map.Width,
                PheromoneRValues = pheromoneColourRValues
            }.ScheduleSingle(this, inputDependencies);
        }

        [BurstCompile]
        private struct Job : IJobForEach<Position, FacingAngle, PheromoneSteering>
        {
            [ReadOnly] public DynamicBuffer<PheromoneColourRValueBuffer> PheromoneRValues;
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
    }
}