using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using UnityEngine;

namespace AntPheromones_ECS
{
    [UpdateInGroup(typeof(PresentationSystemGroup))]
    public class ChangeBrightnessSystem : JobComponentSystem
    {
        private EntityQuery _antRenderingQuery;
        private (bool AreRetrieved, Color Search, Color Carry) _colours;

        protected override void OnCreate()
        {
            base.OnCreate();
            this._antRenderingQuery = GetEntityQuery(ComponentType.ReadOnly<AntRenderingComponent>());
        }
        
        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            if (!_colours.AreRetrieved)
            {
                var antRenderingComponent = this._antRenderingQuery.GetSingleton<AntRenderingComponent>();
                this._colours = 
                    (AreRetrieved: true,
                    Search: antRenderingComponent.SearchColour,
                    Carry: antRenderingComponent.CarryColour);
            }
            
            return new Job
            {
                SearchColour = this._colours.Search,
                CarryColour = this._colours.Carry
            }.Schedule(this, inputDeps);
        }

        [BurstCompile]
        private struct Job : IJobForEach<BrightnessComponent, ResourceCarrierComponent, ColourComponent>
        {
            public Color SearchColour;
            public Color CarryColour;
            
            public void Execute(
                [ReadOnly] ref BrightnessComponent brightness, 
                [ReadOnly] ref ResourceCarrierComponent carrier, 
                [WriteOnly] ref ColourComponent colourDisplay)
            {
                Color colourToDisplay = carrier.IsCarrying ? this.CarryColour : this.SearchColour;
                colourDisplay.Value += (colourToDisplay * brightness.Value - colourDisplay.Value) * 0.05f;
            }
        }
    }
}