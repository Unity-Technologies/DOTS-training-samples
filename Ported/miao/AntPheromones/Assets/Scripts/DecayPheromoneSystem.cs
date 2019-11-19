using Unity.Entities;
using Unity.Jobs;

namespace AntPheromones_ECS
{
    public class DecayPheromoneSystem : JobComponentSystem
    {
        private DynamicBuffer<PheromoneColour> _pheromoneColours;

        private struct Job : IJobParallelFor
        {
            public DynamicBuffer<PheromoneColour> PheromoneColours;
            
            public void Execute(int index)
            {
                PheromoneColour colour = PheromoneColours[index];
                colour.Value.r *= Map.TrailDecayRate;
                PheromoneColours[index] = colour;
            }
        }

        protected override void OnCreate()
        {
            base.OnCreate();
            
            EntityQuery entityQuery = GetEntityQuery(ComponentType.ReadOnly<PheromoneColourMap>());
            Entity pheromoneColourMap = entityQuery.GetSingletonEntity();
            
            BufferFromEntity<PheromoneColour> lookUp = GetBufferFromEntity<PheromoneColour>();
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