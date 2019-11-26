using Unity.Entities;
using Unity.Mathematics;

namespace AntPheromones_ECS
{
    public struct Map : IComponentData
    {
        public int Width;
        public float TrailVisibilityModifier;
        public float TrailDecayRate;

        public float2 ResourcePosition;
        public float2 ColonyPosition;
 
        public BlobAssetReference<Obstacles> Obstacles;
    }
}