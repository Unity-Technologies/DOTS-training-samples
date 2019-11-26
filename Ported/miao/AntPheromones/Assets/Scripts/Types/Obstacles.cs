using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine.UIElements;

namespace AntPheromones_ECS
{
    public struct Obstacles
    {
        public float Radius;
        public BlobArray<float2> Positions;
        
        private int _mapWidth;
        private int _bucketResolution;
        private BlobArray<int> _obstacleBucketIndices;

        public bool HasObstacle(float2 position)
        {
            TryGetObstacles(position, out int _, out int distanceToNextBucket);
            return distanceToNextBucket > 0;
        }

        public void TryGetObstacles(float2 position, out int indexOfCurrentBucket, out int distanceToNextBucket)
        {
            int2 coordinates = (int2) (position / this._mapWidth * this._bucketResolution);

            if (math.any(coordinates < 0) || math.any(coordinates >= this._bucketResolution))
            {
                indexOfCurrentBucket = 0;
                distanceToNextBucket = 0;
            }
            else
            {
                int currentBucket = coordinates.y * this._bucketResolution + coordinates.x;
                int nextBucket = 
                    currentBucket == this._obstacleBucketIndices.Length - 1
                        ? this.Positions.Length
                        : this._obstacleBucketIndices[currentBucket + 1];

                indexOfCurrentBucket = this._obstacleBucketIndices[currentBucket];
                distanceToNextBucket = nextBucket - indexOfCurrentBucket;
            }
        }
        
        public static BlobAssetReference<Obstacles> Build(AntManager antManager)
        {
            float2[,][] obstacleBuckets = antManager.ObstacleBuckets;
            
            using (BlobBuilder builder = new BlobBuilder(Allocator.Temp))
            {
                ref Obstacles obstacles = ref builder.ConstructRoot<Obstacles>();
                obstacles._bucketResolution = antManager.BucketResolution;
                obstacles._mapWidth = antManager.MapWidth;
                
                obstacles.Radius = antManager.ObstacleRadius;
                
                int height = obstacleBuckets.GetLength(0);
                int width = obstacleBuckets.GetLength(1);
                
                int numObstacles = 0;
                
                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        numObstacles += obstacleBuckets[y, x].Length;
                    }
                }
                
                BlobBuilderArray<float2> obstaclePositions = builder.Allocate(ref obstacles.Positions, numObstacles);
                BlobBuilderArray<int> bucketIndices = builder.Allocate(ref obstacles._obstacleBucketIndices, width * height);
                
                int offset = 0;
                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        float2[] bucket = obstacleBuckets[x, y];
                        bucketIndices[y * width + x] = offset;
                        
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