using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class ConfigAuthoring : MonoBehaviour
{
    public int beeCount = 1000;
    public int respawnBeeCount = 10;
    public int foodCount = 10;
    public float bloodDecay = 1.0f;
    public float maxSpawnSpeed = 5f;
    public float2 foodBounds = new float2(10, 10);
    public float3 gravity = new float3(0, -20, 0);
    public GameObject boundsObject;

    public class ConfigBaker : Baker<ConfigAuthoring>
    {
        public override void Bake(ConfigAuthoring config)
        {
            var entity = GetEntity(TransformUsageFlags.None);

            float3 bounds = new float3(10, 10, 10);
            if (config.boundsObject != null)
                bounds = config.boundsObject.transform.localScale * 0.5f;

            Config configComponent = new Config
            {
                beeCount = config.beeCount,
                respawnBeeCount = config.respawnBeeCount,
                foodCount = config.foodCount,
                foodBounds = config.foodBounds,
                bloodDecay = config.bloodDecay,
                maxSpawnSpeed = config.maxSpawnSpeed,
                gravity = config.gravity,
                bounds = bounds
            };

            AddComponent(entity, configComponent);
        }
    }
}