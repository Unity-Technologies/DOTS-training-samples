using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using UnityEngine;

[UpdateInGroup(typeof(PresentationSystemGroup))]
public class UpdateAntColorSystem : JobComponentSystem
{
    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        
    }
    
    struct Job : IJobForEach<BrightnessComponent, HasResourcesComponent, RenderColorComponent>
    {
        public Color SearchColor;
        public Color CarryColor;

        public void Execute(
            [ReadOnly] ref BrightnessComponent brightness,
            [ReadOnly] ref HasResourcesComponent hasResources,
            [WriteOnly] ref RenderColorComponent renderColor)
        {
            var c = hasResources.Value ? CarryColor : SearchColor;
            renderColor.Value += (c * brightness.Value - renderColor.Value) * .05f;
        }
    }
}
