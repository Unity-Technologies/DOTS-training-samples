using Unity.Entities;
using Unity.Mathematics;

namespace AntPheromones_ECS
{
    public struct MapComponent : IComponentData
    {
        public int Width; // = 128;
        public float TrailVisibilityModifier; // = 0.3f;
        public float TrailDecayRate; // = 0.95f;

        public float2 ResourcePosition;
        public float2 ColonyPosition; //= new float2(1f, 1f) * Width * 0.5f;
        
        public float ObstacleRadius;    
        public BlobAssetReference<ObstacleData> Obstacles;
    }
}