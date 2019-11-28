using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace AntPheromones_ECS
{
    public struct Obstacles
    {
        public float Radius { get; private set; }
        public BlobArray<float2> Positions;
        
        private int _mapWidth;
        private int _bucketResolution;
        private BlobArray<int> _obstacleBucketIndices;

        public (bool Exist, int? IndexOfCurrentBucket, int? DistanceToNextBucket) TryGetObstacles(float2 position)
        {
            int2 coordinates = (int2) (position / this._mapWidth * this._bucketResolution);

            if (math.any(coordinates < 0) || math.any(coordinates >= this._bucketResolution))
            {
                return (Exist: true, IndexOfCurrentBucket: 0, DistanceToNextBucket: 0);
            }

            int currentBucket = coordinates.y * this._bucketResolution + coordinates.x;
            int nextBucket = 
                currentBucket == this._obstacleBucketIndices.Length - 1
                    ? this.Positions.Length
                    : this._obstacleBucketIndices[currentBucket + 1];

            int indexOfCurrentBucket = this._obstacleBucketIndices[currentBucket];
            int distanceToNextBucket = nextBucket - indexOfCurrentBucket;
            
            return (Exist: distanceToNextBucket > 0, IndexOfCurrentBucket: indexOfCurrentBucket, DistanceToNextBucket: distanceToNextBucket);
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
                
                BlobBuilderArray<float2> obstaclePositions =
                    builder.Allocate(ref obstacles.Positions, CountObstacles(obstacleBuckets, height, width));
                BlobBuilderArray<int> bucketIndices =
                    builder.Allocate(ref obstacles._obstacleBucketIndices, width * height);
                
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

        private static int CountObstacles(float2[,][] obstacleBuckets, int height, int width)
        {
            int numObstacles = 0;
                
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    numObstacles += obstacleBuckets[y, x].Length;
                }
            }
            return numObstacles;
        }
    }
}