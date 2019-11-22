using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace AntPheromones_ECS
{
    public struct Obstacles
    {
        public int MapWidth;
        public int BucketResolution;
        public float Radius;

        public BlobArray<float2> Positions;
        public BlobArray<bool> HasObstacles;


        public bool HasObstacle(float2 position)
        {
            int2 coordinates = (int2)(position / this.MapWidth * BucketResolution);
            bool isWithinMapBounds = math.all(coordinates >= 0) && math.all(coordinates < this.BucketResolution);

            var temp = coordinates.y * this.BucketResolution + coordinates.x;
            return isWithinMapBounds && this.HasObstacles[temp];
        }
        
        public static BlobAssetReference<Obstacles> Build(AntManager antManager)
        {
            using (BlobBuilder builder = new BlobBuilder(Allocator.Temp))
            {
                ref Obstacles obstacles = ref builder.ConstructRoot<Obstacles>();
                
                obstacles.BucketResolution = antManager.BucketResolution;
                obstacles.MapWidth = antManager.MapWidth;
                obstacles.Radius = antManager.ObstacleRadius;
                
                int height = antManager.ObstacleBuckets.GetLength(0);
                int width = antManager.ObstacleBuckets.GetLength(1);

                BlobBuilderArray<bool> hasObstaclesBuilderArray = 
                    builder.Allocate(ref obstacles.HasObstacles, width * height);

                int offset = 0;

                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        hasObstaclesBuilderArray[offset] = antManager.ObstacleBuckets[y, x].Length > 0;
                        offset++;
                    }
                }

                BlobBuilderArray<float2> obstaclePositionsBuilderArray =
                    builder.Allocate(ref obstacles.Positions, obstacles.MapWidth * obstacles.MapWidth);
                
                for (int i = 0; i < antManager.ObstaclePositions.Length; i++)
                {
                    obstaclePositionsBuilderArray[i] = antManager.ObstaclePositions[i];
                }
                    
                return builder.CreateBlobAssetReference<Obstacles>(Allocator.Persistent);
            }
        }
    }
}