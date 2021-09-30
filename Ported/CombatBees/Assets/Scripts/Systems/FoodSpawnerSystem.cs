using Unity.Entities;
using Unity.Collections;
using Unity.Mathematics;
using Unity.Transforms;


public partial class FoodSpawnerSystem : SystemBase
{
    private Random random;
    protected override void OnCreate()
    {
        base.OnCreate();
        random = new Random((uint)System.DateTime.Now.Ticks);
    }
    
    protected override void OnUpdate()
    {
        var ecb = new EntityCommandBuffer(Allocator.Temp);
        WorldBounds bounds = GetSingleton<WorldBounds>();
        Prefabs prefabs = GetSingleton<Prefabs>();

        uint seed = random.NextUInt();

        Entities
            .ForEach((Entity entity, int entityInQueryIndex, ref FoodSpawner spawner) =>
            {
                // Destroying the current entity is a classic ECS pattern,
                // when something should only be processed once then forgotten.
                if (!spawner.DidSpawn)
                {
                    //check if we are placing food and check if we dropped in a hive
                    if (spawner.PlaceFood)
                    {
                        float3 position = WorldUtils.ClampToWorldBounds(bounds, spawner.SpawnLocation, 0.5f);
                        var instance = ecb.Instantiate(prefabs.FoodPrefab);
                        var translation = new Translation {Value = position};
                        ecb.SetComponent(instance, translation);
                        if (WorldUtils.IsInBlueHive(bounds, spawner.SpawnLocation)|| WorldUtils.IsInRedHive(bounds, spawner.SpawnLocation))
                        {
                            ecb.AddComponent(instance, new InHive(){});
                        }
                        spawner.PlaceFood = false;
                    }
                    else
                    {
                        Random random = new Random((uint) entityInQueryIndex + seed);
                        float3 spawnMin = bounds.AABB.Min;
                        spawnMin.x += bounds.HiveOffset + 50;

                        float3 spawnMax = bounds.AABB.Max;
                        spawnMax.x -= bounds.HiveOffset + 50;
                        for (int i = 0; i < spawner.InitialFoodCount; ++i)
                        {
                            float3 position = random.NextFloat3(spawnMin, spawnMax);
                            position = WorldUtils.ClampToWorldBounds(bounds, position, 0.5f);
                            var instance = ecb.Instantiate(prefabs.FoodPrefab);
                            var translation = new Translation {Value = position};
                            ecb.SetComponent(instance, translation);
                        }
                    }

                    spawner.DidSpawn = true;
                }
            }).Run();

        ecb.Playback(EntityManager);
        ecb.Dispose();
    }
}