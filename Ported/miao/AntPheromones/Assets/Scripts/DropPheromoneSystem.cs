using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

namespace AntPheromones_ECS
{
    public class DropPheromoneSystem : JobComponentSystem
    {
//        private BlobAssetReference<PheromoneColours> PheromoneColours;
        
        private struct Job : IJobForEach<Position, Speed, ResourceCarrier>
        {
            public BlobArray<Color> PheromoneColours;
            
            public void Execute([ReadOnly] ref Position position, [ReadOnly] ref Speed speed, [ReadOnly] ref ResourceCarrier resourceCarrier)
            {
                const float CarrierExcitement = 1f;
                const float SearcherExcitement = 0.3f;

                int2 targetPosition = (int2)math.floor(position.Value);
                
                if (!Map.IsWithinBounds(targetPosition))
                {
                    return;
                }
                
                int index = targetPosition.x + targetPosition.y * Map.Width;
                    
                Color pheromoneColour = this.PheromoneColours[index];
                pheromoneColour.r +=
                    math.min(
                        Map.TrailVisibilityModifier *
                        (resourceCarrier.IsCarrying ? CarrierExcitement : SearcherExcitement) * Time.fixedDeltaTime *
                        (1f - this.PheromoneColours[index].r), 1f);
                
                this.PheromoneColours[index] = pheromoneColour;
            }
        }

        protected override void OnCreate()
        {
//            this.PheromoneColours = AntPheromones_ECS.PheromoneColours.Generate();
        }
       
        protected override JobHandle OnUpdate(JobHandle inputDependencies)
        {
            return new Job().Schedule(this, inputDependencies);
        }
    }
}