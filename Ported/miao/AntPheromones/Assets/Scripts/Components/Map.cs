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
        public float2 ColonyPosition; //= new float2(1f, 1f) * Width * 0.5f;
 
        public BlobAssetReference<Obstacles> Obstacles;
    }
}