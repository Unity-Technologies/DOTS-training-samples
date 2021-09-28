using Unity.Entities;
using Unity.Collections;
using Unity.Mathematics;
using Unity.Transforms;
public partial class FoodSpawnerSystem : SystemBase
{
    private uint Seed;

    protected override void OnCreate()
    {
        Seed = (uint)System.DateTime.Now.Ticks;
    }
    protected override void OnUpdate()
    {
        var ecb = new EntityCommandBuffer(Allocator.Temp);
        var worldBoundsEntity = GetSingletonEntity<WorldBounds>();
        var bounds = GetComponent<WorldBounds>(worldBoundsEntity);
        var random = new Random(Seed);

        Entities
            .ForEach((Entity entity, in FoodSpawner spawner) =>
            {
                // Destroying the current entity is a classic ECS pattern,
                // when something should only be processed once then forgotten.
                ecb.DestroyEntity(entity);
                float3 spawnMin = bounds.AABB.Min;
                spawnMin.x += bounds.HiveOffset;

                float3 spawnMax = bounds.AABB.Max;
                spawnMax.x -= bounds.HiveOffset;

                for (int i = 0; i < spawner.InitialFoodCount; ++i)
                {
                    float3 position = random.NextFloat3(spawnMin, spawnMax);

                    var instance = ecb.Instantiate(spawner.FoodPrefab);
                    var translation = new Translation {Value = position};
                    ecb.SetComponent(instance, translation);
                }
            }).Run();

        ecb.Playback(EntityManager);
        ecb.Dispose();
    }
}