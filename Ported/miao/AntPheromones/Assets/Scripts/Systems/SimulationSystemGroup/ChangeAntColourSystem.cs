using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using UnityEngine;

namespace AntPheromones_ECS
{
    public class ChangeAntColourSystem : JobComponentSystem
    {
        private EntityQuery _antRenderingQuery;
        private (bool AreRetrieved, Color Search, Color Carry) _colours;

        protected override void OnCreate()
        {
            base.OnCreate();
            this._antRenderingQuery = GetEntityQuery(ComponentType.ReadOnly<AntIndividualRendering>());
        }
        
        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            if (!this._colours.AreRetrieved)
            {
                var antRendering = this._antRenderingQuery.GetSingleton<AntIndividualRendering>();
                this._colours = 
                    (AreRetrieved: true,
                    Search: antRendering.SearchColour,
                    Carry: antRendering.CarryColour);
            }
            
            return new Job
            {
                SearchColour = this._colours.Search,
                CarryColour = this._colours.Carry
            }.Schedule(this, inputDeps);
        }

        [BurstCompile]
        private struct Job : IJobForEach<Brightness, ResourceCarrier, Colour>
        {
            public Color SearchColour;
            public Color CarryColour;
            
            public void Execute(
                [ReadOnly] ref Brightness brightness, 
                [ReadOnly] ref ResourceCarrier carrier, 
                [WriteOnly] ref Colour colourDisplay)
            {
                colourDisplay.Value += ((carrier.IsCarrying ? this.CarryColour : this.SearchColour) * brightness.Value - colourDisplay.Value) * 0.05f;
            }
        }
    }
}