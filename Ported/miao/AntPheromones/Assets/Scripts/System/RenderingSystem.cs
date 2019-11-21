using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;

namespace AntPheromones_ECS
{
    public class RenderingSystem : JobComponentSystem
    {
        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            throw new System.NotImplementedException();
        }
        
        private struct Job : IJobForEach<ColourComponent, LocalToWorldComponent>
    }

    public struct LocalToWorldComponent : IComponentData
    {
        public float4x4 Value;
    }
}