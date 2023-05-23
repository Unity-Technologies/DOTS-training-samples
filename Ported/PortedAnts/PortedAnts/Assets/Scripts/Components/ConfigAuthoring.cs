using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Components
{
    public class ConfigAuthoring : MonoBehaviour
    {
        public int ObstacleRingCount = 10;
        public float ObstaclesFillRate = 5;
        public int MapSize = 16;
        public float ObstacleRadius = 1f;
        public GameObject ObstaclePrefab;
        public GameObject AntsTargetPrefab;
        public float4 HomeColor;
        public float4 FoodColor;
        public float4 AntHasFoodColor;
        public float4 AntHasNoFoodColor;
        public float AntSpeed;
        public int AntsPopulation;
        public GameObject AntPrefab;
        public float RandomSteering;
    }

    class ConfigBaker : Baker<ConfigAuthoring>
    {
        public override void Bake(ConfigAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.None);
            AddComponent(entity, new Config
            {
                ObstacleRingCount = authoring.ObstacleRingCount,
                ObstaclesFillRate = authoring.ObstaclesFillRate,
                MapSize = authoring.MapSize,
                ObstacleRadius = authoring.ObstacleRadius,
                ObstaclePrefab = GetEntity(authoring.ObstaclePrefab, TransformUsageFlags.None),
                AntsTargetPrefab = GetEntity(authoring.AntsTargetPrefab, TransformUsageFlags.None),
                HomeColor = authoring.HomeColor,
                FoodColor = authoring.FoodColor,
                AntHasFoodColor = authoring.AntHasFoodColor,
                AntHasNoFoodColor = authoring.AntHasNoFoodColor,
                AntSpeed = authoring.AntSpeed,
                AntsPopulation = authoring.AntsPopulation,
                AntPrefab = GetEntity(authoring.AntPrefab, TransformUsageFlags.None),
                RandomSteering = authoring.RandomSteering
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
        public Entity AntsTargetPrefab;
        public float4 HomeColor;
        public float4 FoodColor;
        public float4 AntHasFoodColor;
        public float4 AntHasNoFoodColor;
        public float AntSpeed;
        public int AntsPopulation;
        public Entity AntPrefab;
        public float RandomSteering;
    }
}
