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
            int2 coordinates = (int2)(position / MapWidth * BucketResolution);
            bool isWithinMapBounds = !math.any(coordinates < 0) && !math.any(coordinates > this.MapWidth);
            
            return isWithinMapBounds && this.HasObstacles[coordinates.y * this.MapWidth + coordinates.x];
        }
    }
}