using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine.UIElements;

namespace AntPheromones_ECS
{
    public struct Obstacles
    {
        public int MapWidth;
        public int BucketResolution;
        public float Radius;

        public BlobArray<float2> Positions;
        public BlobArray<int> ObstacleBucketIndices;

        public bool HasObstacle(float2 position)
        {
            int2 coordinates = (int2)(position / this.MapWidth * BucketResolution);
            bool isWithinMapBounds = math.all(coordinates >= 0) && math.all(coordinates < this.BucketResolution);

            TryGetObstacles(position, out int _, out int length);
            return isWithinMapBounds && length > 0;
        }

        public void TryGetObstacles(float2 position, out int offset, out int distanceToNextBucket)
        {
            int2 coordinates = (int2) (position / this.MapWidth * this.BucketResolution);

            if (math.any(coordinates < 0) || math.any(coordinates >= this.BucketResolution))
            {
                offset = 0;
                distanceToNextBucket = 0;
            }

            int currentBucket = coordinates.y * this.BucketResolution + coordinates.x;
            int nextBucket = 
                currentBucket == this.ObstacleBucketIndices.Length - 1
                    ? this.Positions.Length
                    : this.ObstacleBucketIndices[currentBucket + 1];

            offset = this.ObstacleBucketIndices[currentBucket];
            distanceToNextBucket = nextBucket - offset;
        }
        
        public static BlobAssetReference<Obstacles> Build(AntManager antManager)
        {
            var obstacleMap = antManager.ObstacleBuckets;
            using (BlobBuilder builder = new BlobBuilder(Allocator.Temp))
            {
                ref Obstacles obstacles = ref builder.ConstructRoot<Obstacles>();
                obstacles.BucketResolution = antManager.BucketResolution;
                obstacles.MapWidth = antManager.MapWidth;

                int h = obstacleMap.GetLength(0);
                int w = obstacleMap.GetLength(1);
                
                int totalSize = 0;
                
                for (int y = 0; y < h; y++)
                {
                    for (int x = 0; x < w; x++)
                        totalSize += obstacleMap[y, x].Length;
                }
                
                BlobBuilderArray<float2> obstaclePositions = builder.Allocate(ref obstacles.Positions, totalSize);
                BlobBuilderArray<int> bucketIndices = builder.Allocate(ref obstacles.ObstacleBucketIndices, w * h);
                
                int offset = 0;
                for (int y = 0; y < h; y++)
                {
                    for (int x = 0; x < w; x++)
                    {
                        float2[] bucket = obstacleMap[x, y];
                        bucketIndices[y * w + x] = offset;
                        for (int i = 0; i < bucket.Length; i++)
                        {
                            obstaclePositions[offset] = bucket[i];
                            offset++;
                        }
                    }
                }
                return builder.CreateBlobAssetReference<Obstacles>(Allocator.Persistent);
            }
        }
    }
}