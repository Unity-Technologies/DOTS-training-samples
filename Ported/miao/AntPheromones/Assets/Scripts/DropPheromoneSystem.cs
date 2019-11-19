using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using UnityEngine;

namespace AntPheromones_ECS
{
    public class DropPheromoneSystem : JobComponentSystem
    {
        private BlobAssetReference<PheromoneColourBlobs> PheromoneColours;
        
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
            this.PheromoneColours = PheromoneColourBlobs.Generate();
        }
       
        protected override JobHandle OnUpdate(JobHandle inputDependencies)
        {
            
            throw new NotImplementedException();
//            DropPheromoneJob job = new DropPheromoneJob { DropForce = inputDependencies.};
        }
    }

    public struct PheromoneColourBlobs
    {
        public BlobArray<Color> Colours;

        public static BlobAssetReference<PheromoneColourBlobs> Generate()
        {
            using (var builder = new BlobBuilder(Allocator.Temp))
            {
                ref PheromoneColourBlobs pheromoneColourBlobs = ref builder.ConstructRoot<PheromoneColourBlobs>();
                BlobBuilderArray<Color> blobBuilderArray = builder.Allocate(ref pheromoneColourBlobs.Colours, length: Map.Width * Map.Width);

                for (int i = 0; i < pheromoneColourBlobs.Colours.Length; i++)
                {
                    blobBuilderArray[i] = new Color();
                }

                return builder.CreateBlobAssetReference<PheromoneColourBlobs>(Allocator.Persistent);
            }
        }
    }
}