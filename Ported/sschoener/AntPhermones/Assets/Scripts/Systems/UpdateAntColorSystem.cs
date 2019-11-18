using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;

[UpdateInGroup(typeof(PresentationSystemGroup))]
public class UpdateAntColorSystem : JobComponentSystem
{
    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        throw new System.NotImplementedException();
    }
    
    struct UpdateSearchColorJob : IJobForEach<BrightnessComponent, RenderColorComponent>
    {
        public void Execute([ReadOnly]ref BrightnessComponent brightness, [WriteOnly] ref RenderColorComponent renderColor)
        {
        }
    }
    
    struct UpdateCarryColorJob : IJobForEach<BrightnessComponent, RenderColorComponent>
    {
        public void Execute([ReadOnly]ref BrightnessComponent brightness, [WriteOnly] ref RenderColorComponent renderColor)
        {
        }
    }
}
