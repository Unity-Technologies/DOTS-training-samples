using Unity.Entities;
using Unity.Jobs;

namespace AntPheromones_ECS
{
    public class DecayPheromoneSystem : JobComponentSystem
    {
        private DynamicBuffer<PheromoneColourRValue> _pheromoneColours;

        private struct Job : IJobParallelFor
        {
            public DynamicBuffer<PheromoneColourRValue> PheromoneColours;
            
            public void Execute(int index)
            {
                PheromoneColourRValue colourRValue = PheromoneColours[index];
                colourRValue.Value.r *= Map.TrailDecayRate;
                PheromoneColours[index] = colourRValue;
            }
        }

        protected override void OnCreate()
        {
            base.OnCreate();
            
            EntityQuery entityQuery = GetEntityQuery(ComponentType.ReadOnly<PheromoneColourMap>());
            Entity pheromoneColourMap = entityQuery.GetSingletonEntity();
            
            BufferFromEntity<PheromoneColourRValue> lookUp = GetBufferFromEntity<PheromoneColourRValue>();
            this._pheromoneColours = lookUp[pheromoneColourMap];
        }

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        { 
            return new Job
            {
                PheromoneColours = this._pheromoneColours
            }.Schedule(arrayLength: Map.Width,
                innerloopBatchCount: MapObstacles.BucketResolution,
                inputDeps);  
        }
    }
}