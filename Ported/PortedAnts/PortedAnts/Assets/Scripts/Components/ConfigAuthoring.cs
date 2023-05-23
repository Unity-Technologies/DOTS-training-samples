using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Serialization;

namespace Components
{
    public class ConfigAuthoring : MonoBehaviour
    {
        public int ObstacleRingCount = 10;
        public float ObstaclesFillRate = 5;
        public int MapSize = 16;
        public float ObstacleRadius = 1f;
        public GameObject ObstaclePrefab;
    }

    class ConfigBaker : Baker<ConfigAuthoring>
    {
        public override void Bake(ConfigAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Renderable);
            AddComponent(entity, new Config
            {
                ObstacleRingCount = authoring.ObstacleRingCount,
                ObstaclesFillRate = authoring.ObstaclesFillRate,
                MapSize = authoring.MapSize,
                ObstacleRadius = authoring.ObstacleRadius,
                ObstaclePrefab = GetEntity(authoring.ObstaclePrefab, TransformUsageFlags.Renderable),
            });
        }
    }

    public struct Config : IComponentData
    {
        public int ObstacleRingCount;
        public float ObstaclesFillRate;
        public int MapSize;
        public float ObstacleRadius;
        public Entity ObstaclePrefab;

    }
}
