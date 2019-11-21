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
        private DynamicBuffer<PheromoneColourRValue> _pheromoneColourRValues;
        private MapComponent _map;

        protected override void OnCreate()
        {
            base.OnCreate();
            
            this._map =
                GetEntityQuery(ComponentType.ReadOnly<MapComponent>()).GetSingleton<MapComponent>();
            
            Entity pheromoneRValues = GetEntityQuery(ComponentType.ReadOnly<PheromoneColourRValue>()).GetSingletonEntity();
            this._pheromoneColourRValues = GetBufferFromEntity<PheromoneColourRValue>(isReadOnly: true)[pheromoneRValues];
        }

        private struct Job : IJobForEach<PositionComponent, FacingAngleComponent, PheromoneSteeringComponent>
        {
            public DynamicBuffer<PheromoneColourRValue> PheromoneRValues;
            public int MapWidth;

            public void Execute(
                [ReadOnly] ref PositionComponent position,
                [ReadOnly] ref FacingAngleComponent facingAngleComponent,
                ref PheromoneSteeringComponent pheromoneSteeringComponent)
            {
                const float Distance = 3;

                float result = 0;

                for (int i = -1; i <= 1; i += 2)
                {
                    float angle = facingAngleComponent.Value + i * Mathf.PI * 0.25f;
                    int candidateDestinationX = (int)(position.Value.x + Mathf.Cos(angle) * Distance);
                    int candidateDestinationY = (int)(position.Value.y + Mathf.Sin(angle) * Distance);

                    if (candidateDestinationX < 0 || candidateDestinationY < 0 ||
                        candidateDestinationX >= this.MapWidth || candidateDestinationY >= this.MapWidth)
                    {
                        continue;
                    }
                    int pheromoneIndex = candidateDestinationX + candidateDestinationY * this.MapWidth;
                    result += this.PheromoneRValues[pheromoneIndex] * i;
                }

                pheromoneSteeringComponent.Value = result;
            }
        }

        protected override JobHandle OnUpdate(JobHandle inputDependencies)
        {
            return new Job
            {
                MapWidth = this._map.Width,
                PheromoneRValues = this._pheromoneColourRValues
            }.Schedule(this, inputDependencies);
        }
    }
}
