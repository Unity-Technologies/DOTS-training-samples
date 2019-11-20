using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace AntPheromones_ECS
{
    public class MapAuthoringComponent : MonoBehaviour, IConvertGameObjectToEntity
    {
        [Range(0f, 1f)] public float TrailVisibilityModifier = 0.3f;
        [Range(0f, 1f)] public float TrailDecayRate = 0.95f;
        
        public void Convert(Entity entity, EntityManager entityManager, GameObjectConversionSystem conversionSystem)
        {
            entityManager.AddComponentData(entity, new MapComponent
            {
                Width = AntManager.Instance.MapWidth,
                ColonyPosition = AntManager.Instance.ColonyPosition,
                ResourcePosition = AntManager.Instance.ResourcePosition,
                ObstacleRadius = AntManager.Instance.ObstacleRadius,
                TrailDecayRate = this.TrailDecayRate,
                TrailVisibilityModifier = this.TrailVisibilityModifier,
                Obstacles = CreateObstacles()
            });
        }

        private BlobAssetReference<ObstacleData> CreateObstacles()
        {
            float2[] obstaclePositions = AntManager.Instance.ObstaclePositions;
            float2[,][] obstacleBuckets = AntManager.Instance.ObstacleBuckets;
            
            using (BlobBuilder builder = new BlobBuilder(Allocator.Temp))
            {
                ref ObstacleData obstacleData = ref builder.ConstructRoot<ObstacleData>();
                
                obstacleData.BucketResolution = AntManager.Instance.BucketResolution;
                obstacleData.MapWidth = AntManager.Instance.MapWidth;
                
                int height = obstacleBuckets.GetLength(0);
                int width = obstacleBuckets.GetLength(1);

                BlobBuilderArray<bool> hasObstaclesBuilderArray = 
                    builder.Allocate(ref obstacleData.HasObstacles, width * height);

                int offset = 0;

                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        hasObstaclesBuilderArray[offset] = obstacleBuckets[y, x].Length > 0;
                        offset++;
                    }
                }

                BlobBuilderArray<float2> obstaclePositionsBuilderArray =
                    builder.Allocate(ref obstacleData.Positions, obstacleData.MapWidth * obstacleData.MapWidth);
                
                for (int i = 0; i < obstaclePositions.Length; i++)
                {
                    obstaclePositionsBuilderArray[i] = obstaclePositions[i];
                }
                    
                return builder.CreateBlobAssetReference<ObstacleData>(Allocator.Persistent);
            }
        }
    }
}