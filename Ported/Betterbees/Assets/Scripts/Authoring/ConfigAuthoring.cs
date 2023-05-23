using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class ConfigAuthoring : MonoBehaviour
{
    public int beeCount = 10;
    public int foodCount = 10;
    public float bloodDecay = 1.0f;
    public float maxSpawnSpeed = 5f;
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
                foodCount = config.foodCount,
                bloodDecay = config.bloodDecay,
                maxSpawnSpeed = config.maxSpawnSpeed,
                gravity = config.gravity,
                bounds = bounds
            };

            AddComponent(entity, configComponent);
        }
    }
}