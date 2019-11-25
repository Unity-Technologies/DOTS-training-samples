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

            return isWithinMapBounds && TryGetObstacles(position).HasObstacles;
        }

        public (bool HasObstacles, int? DistanceToNextBucket) TryGetObstacles(float2 position)
        {
            int2 coordinates = (int2) (position / this.MapWidth * this.BucketResolution);

            if (math.any(coordinates < 0) || math.any(coordinates >= this.BucketResolution))
            {
                return (HasObstacles: false, DistanceToNextBucket: null);
            }

            int currentBucket = coordinates.y * this.BucketResolution + coordinates.x;
            int nextBucket = 
                currentBucket == this.ObstacleBucketIndices.Length - 1
                    ? this.Positions.Length
                    : this.ObstacleBucketIndices[currentBucket + 1];
            int distanceToNextBucket = nextBucket - this.ObstacleBucketIndices[currentBucket];

            return (HasObstacles: distanceToNextBucket > 0, DistanceToNextBucket: distanceToNextBucket);
        }
        
        public static BlobAssetReference<Obstacles> Build(AntManager antManager)
        {
            var obstacleMap = antManager.ObstacleBuckets;
            using (BlobBuilder b = new BlobBuilder(Allocator.Temp))
            {
                ref var obstacles = ref b.ConstructRoot<Obstacles>();
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

                {
                    var obstacles = b.Allocate(ref obstacles.Obstacles, totalSize);
                    var map = b.Allocate(ref obstacles.ObstacleBucketIndices, w * h);
                    int offset = 0;
                    for (int y = 0; y < h; y++)
                    {
                        for (int x = 0; x < w; x++)
                        {
                            var bucket = obstacleMap[x, y];
                            int n = bucket.Length;
                            map[y * w + x] = offset;
                            for (int i = 0; i < n; i++)
                            {
                                obstacles[offset] = bucket[i];
                                offset++;
                            }
                        }
                    }
                }

                return b.CreateBlobAssetReference<ObstacleData>(Allocator.Persistent);
            }
        }
    }
}