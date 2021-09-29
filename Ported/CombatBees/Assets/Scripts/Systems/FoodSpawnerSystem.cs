using Unity.Entities;
using Unity.Collections;
using Unity.Mathematics;
using Unity.Transforms;


public partial class FoodSpawnerSystem : SystemBase
{
    protected override void OnUpdate()
    {
        var ecb = new EntityCommandBuffer(Allocator.Temp);
        var worldBoundsEntity = GetSingletonEntity<WorldBounds>();
        var bounds = GetComponent<WorldBounds>(worldBoundsEntity); 

        Entities
            .ForEach((Entity entity, int entityInQueryIndex, ref FoodSpawner spawner) =>
            {
                // Destroying the current entity is a classic ECS pattern,
                // when something should only be processed once then forgotten.
                if (!spawner.DidSpawn)
                {
                    Random random = new Random((uint)entityInQueryIndex + 1);    
                    float3 spawnMin = bounds.AABB.Min;
                    spawnMin.x += bounds.HiveOffset + 50;

                    float3 spawnMax = bounds.AABB.Max;
                    spawnMax.x -= bounds.HiveOffset + 50;
                    for (int i = 0; i < spawner.InitialFoodCount; ++i)
                    {
                        float3 position = random.NextFloat3(spawnMin, spawnMax);

                        var instance = ecb.Instantiate(spawner.FoodPrefab);
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