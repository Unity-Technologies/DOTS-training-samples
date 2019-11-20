using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace AntPheromones_ECS
{
    public class MapAuthoring : MonoBehaviour, IConvertGameObjectToEntity
    {
        public float TrailVisibilityModifier = 0.3f; 
        public float TrailDecayRate = 0.95f;
        
        public void Convert(Entity entity, EntityManager entityManager, GameObjectConversionSystem conversionSystem)
        {
            entityManager.AddComponentData(entity, new Map
            {
                Width = AntManager.Instance.MapWidth,
                ColonyPosition = AntManager.Instance.ColonyPosition,
                ResourcePosition = AntManager.Instance.ResourcePosition,
                ObstacleRadius = AntManager.Instance.ObstacleRadius,
                TrailDecayRate = this.TrailDecayRate,
                TrailVisibilityModifier = this.TrailVisibilityModifier,
                Obstacles = BuildObstacles();
            });
        }

        private BlobAssetReference<ObstacleData> BuildObstacles()
        {
            var obstacleList = AntManager.Instance.Obstacles;
            var obstacleMap = AntManager.Instance.ObstacleBuckets;
            using (BlobBuilder b = new BlobBuilder(Allocator.Temp))
            {
                ref var obs = ref b.ConstructRoot<ObstacleData>();
                obs.BucketResolution = GameManager.bucketResolution;
                obs.MapSize = GameManager.mapSize;

                {
                    int h = obstacleMap.GetLength(0);
                    int w = obstacleMap.GetLength(1);
                    var map = b.Allocate(ref obs.ObstacleMap, w * h);
                    int offset = 0;
                    for (int y = 0; y < h; y++)
                    {
                        for (int x = 0; x < w; x++)
                        {
                            map[offset] = obstacleMap[y, x].Length > 0;
                            offset++;
                        }
                    }
                }

                var list = b.Allocate(ref obs.Obstacles, obs.MapSize * obs.MapSize);
                for (int i = 0; i < obstacleList.Length; i++)
                    list[i] = obstacleList[i];

                return b.CreateBlobAssetReference<ObstacleData>(Allocator.Persistent);
            }
        }
    }
}