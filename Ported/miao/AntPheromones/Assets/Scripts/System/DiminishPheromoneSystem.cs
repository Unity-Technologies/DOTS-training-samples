using Unity.Entities;
using Unity.Jobs;

namespace AntPheromones_ECS
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateAfter(typeof(DropPheromoneSystem))]
    public class DecayPheromoneSystem : JobComponentSystem
    {
        private DynamicBuffer<PheromoneColourRValue> _pheromoneColourRValues;
        private MapComponent _map;

        private struct Job : IJobParallelFor
        {
            public DynamicBuffer<PheromoneColourRValue> PheromoneColours;
            public float TrailDecayRate;
            
            public void Execute(int index)
            {
                PheromoneColourRValue colourRValue = PheromoneColours[index];
                colourRValue *= this.TrailDecayRate;
                
                PheromoneColours[index] = colourRValue;
            }
        }

        protected override void OnCreate()
        {
            base.OnCreate();
            this._map = GetEntityQuery(ComponentType.ReadOnly<MapComponent>()).GetSingleton<MapComponent>();
            
            Entity pheromoneRValues = GetEntityQuery(ComponentType.ReadWrite<PheromoneColourRValue>()).GetSingletonEntity();
            this._pheromoneColourRValues = GetBufferFromEntity<PheromoneColourRValue>(isReadOnly: true)[pheromoneRValues];
        }

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            return new Job
            {
                TrailDecayRate = this._map.TrailDecayRate,
                PheromoneColours = this._pheromoneColourRValues
            }.Schedule(arrayLength: this._map.Width,
                innerloopBatchCount: 64, // Heuristically determined
                inputDeps);  
        }
    }
}