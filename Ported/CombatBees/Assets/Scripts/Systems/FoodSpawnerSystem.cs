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
                    Random random = new Random((uint)entityInQueryIndex + seed);    
                    float3 spawnMin = bounds.AABB.Min;
                    spawnMin.x += bounds.HiveOffset + 50;

                    float3 spawnMax = bounds.AABB.Max;
                    spawnMax.x -= bounds.HiveOffset + 50;
                    for (int i = 0; i < spawner.InitialFoodCount; ++i)
                    {
                        float3 position = random.NextFloat3(spawnMin, spawnMax);

                        var instance = ecb.Instantiate(prefabs.FoodPrefab);
                        var translation = new Translation {Value = position};
                        ecb.SetComponent(instance, translation);
                    }

                    spawner.DidSpawn = true;
                }
            }).Run();

        ecb.Playback(EntityManager);
        ecb.Dispose();
    }
}