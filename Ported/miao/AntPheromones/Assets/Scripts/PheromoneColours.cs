using Unity.Entities;
using UnityEngine;

namespace AntPheromones_ECS
{
    public struct PheromoneColour : IBufferElementData
    {
        public Color Value; 
        
        public static implicit operator PheromoneColour(Color colour) 
            => new PheromoneColour { Value = colour };
        
        public static implicit operator Color(PheromoneColour buffer) 
            => buffer.Value;
    }
    
//    public struct PheromoneColours
//    {
//        public BlobArray<Color> Colours;
//
//        public static BlobAssetReference<PheromoneColours> Generate()
//        {
//            using (var builder = new BlobBuilder(Allocator.Temp))
//            {
//                ref PheromoneColours pheromoneColours = ref builder.ConstructRoot<PheromoneColours>();
//                BlobBuilderArray<Color> blobBuilderArray = builder.Allocate(ref pheromoneColours.Colours, length: Map.Width * Map.Width);
//
//                for (int i = 0; i < pheromoneColours.Colours.Length; i++)
//                {
//                    blobBuilderArray[i] = new Color();
//                }
//
//                return builder.CreateBlobAssetReference<PheromoneColours>(Allocator.Persistent);
//            }
//        }
//    }
}