using Unity.Entities;
using Unity.Jobs;
using UnityEngine;

namespace AntPheromones_ECS
{
    public class RandomizeFacingAngleSystem : JobComponentSystem
    {
        private struct Job : IJobForEach<FacingAngleComponent>
        {
            public void Execute(ref FacingAngleComponent facingAngleComponent)
            {
                facingAngleComponent.Value += Random.Range(-0.14f, 0.14f);
            }
        }
        
        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            return new Job().Schedule(this, inputDeps);
        }
    }
}