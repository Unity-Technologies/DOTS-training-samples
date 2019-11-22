using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using UnityEngine;

namespace AntPheromones_ECS
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateAfter(typeof(RandomizeFacingAngleSystem))]
    public class CalculatePheromoneSteeringSystem : JobComponentSystem
    {
        private EntityQuery _mapQuery;

        protected override void OnCreate()
        {
            base.OnCreate();
            this._mapQuery = GetEntityQuery(ComponentType.ReadOnly<MapComponent>());
        }

        protected override JobHandle OnUpdate(JobHandle inputDependencies)
        {
            var map = this._mapQuery.GetSingleton<MapComponent>();
            
            Entity pheromoneRValues = 
                GetEntityQuery(ComponentType.ReadOnly<PheromoneColourRValueBuffer>()).GetSingletonEntity();
            var pheromoneColourRValues = GetBufferFromEntity<PheromoneColourRValueBuffer>(isReadOnly: true)[pheromoneRValues];
            
            return new Job
            {
                MapWidth = map.Width,
                PheromoneRValues = pheromoneColourRValues.AsNativeArray()
            }.ScheduleSingle(this, inputDependencies);
        }

        [BurstCompile]
        private struct Job : IJobForEach<PositionComponent, FacingAngleComponent, PheromoneSteeringComponent>
        {
            public NativeArray<PheromoneColourRValueBuffer> PheromoneRValues;
            public int MapWidth;

            public void Execute(
                [ReadOnly] ref PositionComponent position,
                [ReadOnly] ref FacingAngleComponent facingAngleComponent,
                [WriteOnly] ref PheromoneSteeringComponent steering)
            {
                const float Distance = 3;

                float result = 0;

                for (int i = 0; i <= 1; i++)
                {
                    float angle = facingAngleComponent.Value + i * Mathf.PI * 0.25f;
                    int targetDestinationX = (int)(position.Value.x + Mathf.Cos(angle) * Distance);
                    int targetDestinationY = (int)(position.Value.y + Mathf.Sin(angle) * Distance);

                    if (targetDestinationX < 0 || targetDestinationY < 0 ||
                        targetDestinationX >= this.MapWidth || targetDestinationY >= this.MapWidth)
                    {
                        continue;
                    }
                    int pheromoneIndex = targetDestinationX + targetDestinationY * this.MapWidth;
                    result += this.PheromoneRValues[pheromoneIndex] * i;
                }

                steering.Value = result >= 0 ? 1 : -1;
            }
        }
    }
}