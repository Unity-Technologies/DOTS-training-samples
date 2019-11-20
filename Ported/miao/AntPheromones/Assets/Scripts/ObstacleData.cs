using Unity.Entities;
using Unity.Mathematics;

namespace AntPheromones_ECS
{
    public struct ObstacleData
    {
        public int MapWidth;
        public int BucketResolution;
        public BlobArray<float2> Positions;
        public BlobArray<bool> HasObstacles;

        public bool HasObstacle(float2 position)
        {
            float2 coordinates = position / MapWidth * BucketResolution;
            bool isWithinMapBounds = !math.any(coordinates < 0) && !math.any(coordinates > this.MapWidth);
            
            return isWithinMapBounds && this.HasObstacles[(int) position.y * this.MapWidth + (int) position.x];
        }
    }
}