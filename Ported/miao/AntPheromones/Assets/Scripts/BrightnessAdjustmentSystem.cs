using Unity.Entities;
using Unity.Jobs;

namespace AntPheromones_ECS
{
    public class BrightnessAdjustmentSystem : JobComponentSystem
    {
        private struct AdjustBrightnessJob : IJobForEach<Brightness, ResourceCarrier>
        {
            public void Execute(ref Brightness brightness, ref ResourceCarrier resourceCarrier)
            {
                if (resourceCarrier.IsCarrying)
                {
                    
                }
            }
        }
        
        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            throw new System.NotImplementedException();
        }
    }
}