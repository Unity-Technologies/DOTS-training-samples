using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

namespace AntPheromones_ECS
{
    public struct ObstacleBlobs
    {
        public const int MapWidth = 128;
        
        public BlobArray<BlobArray<Obstacle>> ObstacleBuckets;
        public BlobArray<Obstacle> Empty;
        public BlobArray<Obstacle> Obstacles;
        private BlobArray<Matrix4x4> ObstacleMatrices;
        
        private const int BucketResolution = 64;
        private const int ObstacleRingCount = 3;
        private const int ObstacleRadius = 2;
        private const float NumObstaclesPerRing = 0.8f;
        private const int MaxHoleCount = 3;
        private const int MinHoleCount = 1;

        public static BlobAssetReference<ObstacleBlobs> GenerateObstacles()
        {
            using (BlobBuilder builder = new BlobBuilder(Allocator.Temp))
            {
                ref ObstacleBlobs root = ref builder.ConstructRoot<ObstacleBlobs>();

                builder.Allocate(ref root.Empty, length: 0);
                
                BlobBuilderArray<Obstacle> obstacles = builder.Allocate(ref root.Obstacles, length: 250);

                for (int i = 0; i < ObstacleRingCount; i++)
                {
                    float ringRadius = i / (ObstacleRingCount + 1f) * (MapWidth * .5f);
                    float circumference = ringRadius * 2f * Mathf.PI;

                    int maxObstacleCount = Mathf.CeilToInt(circumference / (2f * ObstacleRadius) * 2f);

                    int offset = Random.Range(0, maxObstacleCount);
                    int holeCount = Random.Range(MinHoleCount, MaxHoleCount);

                    for (int j = 0; j < maxObstacleCount; j++)
                    {
                        float t = (float) j / maxObstacleCount;

                        if (t * holeCount % 1f >= NumObstaclesPerRing)
                        {
                            continue;
                        }
                        
                        float angle = (j + offset) / (float) maxObstacleCount * (2f * Mathf.PI);
                        obstacles[i] = new Obstacle
                        {
                            Position = 
                                new float3(
                                    MapWidth * 0.5f + Mathf.Cos(angle) * ringRadius, 
                                    MapWidth * .5f + Mathf.Sin(angle) * ringRadius,
                                    0),
                            Radius = ObstacleRadius
                        };
                    }
                }

                BlobBuilderArray<Matrix4x4> obstacleMatrices = builder.Allocate(ref root.ObstacleMatrices, length: obstacles.Length);

                for (int i = 0; i < obstacleMatrices.Length; i++)
                {
                    obstacleMatrices[i] = Unity.Mathematics.float4x4.TRS(
                            obstacles[i].Position / MapWidth, Quaternion.identity,
                            new float3(ObstacleRadius * 2f, ObstacleRadius * 2f, 1f) / MapWidth);
                }

                BlobBuilderArray<BlobArray<Obstacle>> obstacleBuckets = 
                    builder.Allocate(ref root.ObstacleBuckets, length: BucketResolution * BucketResolution);

                for (int i = 0; i < obstacleBuckets.Length; i++)
                {
                    obstacleBuckets[i] = new BlobArray<Obstacle>();
                }

                for (int i = 0; i < obstacles.Length; i++)
                {
                    float3 position = obstacles[i].Position;
                    float radius = obstacles[i].Radius;

                    for (int x = Mathf.FloorToInt((position.x - radius) / MapWidth * BucketResolution);
                        x <= Mathf.FloorToInt((position.x + radius) / MapWidth * BucketResolution);
                        x++)
                    {
                        if (x < 0 || x >= BucketResolution)
                        {
                            continue;
                        }

                        int currentBucketIndex = x * BucketResolution + BucketResolution;
                        int currentBlobArrayWriteIndex = 0;
                        
                        for (int y = Mathf.FloorToInt((position.y - radius) / MapWidth * BucketResolution);
                            y <= Mathf.FloorToInt((position.y + radius) / MapWidth * BucketResolution);
                            y++)
                        {
                            if (y < 0 || y >= BucketResolution)
                            {
                                continue;
                            }

                            obstacleBuckets[currentBucketIndex][currentBlobArrayWriteIndex] = obstacles[i];
                            currentBucketIndex++;
                        }
                    }
                }

                return builder.CreateBlobAssetReference<ObstacleBlobs>(Allocator.Persistent);
            }
        }

        public BlobArray<Obstacle> GetObstacleBucket(float candidateDestinationX, float candidateDestinationY)
        {
            int x = (int) (candidateDestinationX / MapWidth * BucketResolution);
            int y = (int) (candidateDestinationY / MapWidth * BucketResolution);
            
            return IsWithinBounds(x, y) ? Empty : this.ObstacleBuckets[x * BucketResolution + BucketResolution];
        }

        public static bool IsWithinBounds(int positionX, int positionY)
        {
            return positionX >= 0 && positionY >= 0 && positionX < MapWidth && positionY < MapWidth;
        }
    }
}