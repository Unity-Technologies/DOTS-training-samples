using Unity.Entities;
using Unity.Mathematics;

namespace AntPheromones_ECS
{
    public struct Map : IComponentData
    {
        public int Width; // = 128;
        public float TrailVisibilityModifier; // = 0.3f;
        public float TrailDecayRate; // = 0.95f;

        public float2 ResourcePosition;
        public float2 ColonyPosition; //= new float2(1f, 1f) * Width * 0.5f;
        
        public float ObstacleRadius;    
        public BlobAssetReference<ObstacleData> Obstacles;
//        
//        public static bool IsWithinBounds(int positionX, int positionY)
//        {
//            return positionX >= 0 && positionY >= 0 && positionX < Width && positionY < Width;
//        }
//
//        public static bool IsWithinBounds(int2 position)
//        {
//            return math.any(position < 0) || math.any(position >= Width);
//        }
    }

    public struct ObstacleData
    {
        public int BucketResolution;
        public int MapWidth;
        public BlobArray<float2> Obstacles;
        public BlobArray<bool> ObstacleMap;

        public bool HasObstacle(float2 pos)
        {
            var c = pos / MapWidth * BucketResolution;
            return !math.any(c < 0) && !math.any(c > MapWidth) && ObstacleMap[(int) pos.y * MapWidth + (int) pos.x];
        }
    }
}