using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using UnityEngine;

namespace AntPheromones_ECS
{
    public class DropPheromoneSystem : JobComponentSystem
    {
        private BlobAssetReference<ColourBlobs> PheromoneColours;
        
        private struct DropPheromoneJob : IJobForEach<Position, Ant>
        {
            public BlobArray<Color> PheromoneColours;
            public float DropForce;
            public float TrailAddSpeed; // 0.3
            
            public void Execute(ref Position position, ref Ant _)
            {
                int x = Mathf.FloorToInt(position.Value.x);
                int y = Mathf.FloorToInt(position.Value.y);

                if (!Map.IsWithinBounds(x, y))
                {
                    return;
                }
                
                int index = x + y * Map.Width;
                    
                Color pheromoneColour = this.PheromoneColours[index];
                pheromoneColour.r += this.TrailAddSpeed * this.DropForce * Time.fixedDeltaTime *
                                     (1f - this.PheromoneColours[index].r);

                if (pheromoneColour.r > 1f)
                {
                    pheromoneColour.r = 1f;
                }
                    
                this.PheromoneColours[index] = pheromoneColour;
            }
        }

        protected override void OnCreate()
        {
            this.PheromoneColours = ColourBlobs.Generate();
        }
       
        protected override JobHandle OnUpdate(JobHandle inputDependencies)
        {
            DropPheromoneJob job = new DropPheromoneJob { DropForce = };
        }
    }

    public struct ColourBlobs
    {
        public BlobArray<Color> Colours;

        public static BlobAssetReference<ColourBlobs> Generate()
        {
            using (var builder = new BlobBuilder(Allocator.Temp))
            {
                ref ColourBlobs colourBlobs = ref builder.ConstructRoot<ColourBlobs>();
                BlobBuilderArray<Color> blobBuilderArray = builder.Allocate(ref colourBlobs.Colours, length: Map.Width * Map.Width);

                return builder.CreateBlobAssetReference<ColourBlobs>(Allocator.Persistent);
            }
        }
    }
}