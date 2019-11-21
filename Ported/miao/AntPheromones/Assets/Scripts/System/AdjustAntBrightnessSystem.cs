using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using UnityEngine;

namespace AntPheromones_ECS
{
    public class AdjustAntBrightnessSystem : JobComponentSystem
    {
        private struct Job : IJobForEach<BrightnessComponent, ResourceCarrierComponent, ColourComponent>
        {
            public Color SearchColour;
            public Color CarryColour;
            
            public void Execute(
                [ReadOnly] ref BrightnessComponent brightness, 
                [ReadOnly] ref ResourceCarrierComponent carrier, 
                [WriteOnly] ref ColourComponent colourDisplay)
            {
                Color colourToDisplay = carrier.IsCarrying ? this.SearchColour : this.CarryColour;
                colourDisplay.Value += (colourToDisplay * brightness.Value - colourDisplay.Value) * 0.05f;
            }
        }
        
        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            return new Job
            {
                SearchColour = new Color(48f, 54f, 90f), CarryColour = new Color(184f, 181f, 101f),
//                CarryColour = new Color()
            }.Schedule(this, inputDeps);
        }
    }
}