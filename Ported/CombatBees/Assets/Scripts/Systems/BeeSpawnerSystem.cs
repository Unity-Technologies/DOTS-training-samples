using Unity.Entities;
using Unity.Collections;
using Unity.Mathematics;
using Unity.Transforms;

public partial class BeeSpawnerSystem : SystemBase
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
        // In case we need random (not using .Run()), may need to seed with int entityInQueryIndex, 

        Entities
            .ForEach((Entity entity, in BeeSpawner spawner) =>
            {
                // Destroying the current entity is a classic ECS pattern,
                // when something should only be processed once then forgotten.
                ecb.DestroyEntity(entity);
                float3 blueSpawnMax = bounds.AABB.Min;
                blueSpawnMax.x += bounds.HiveOffset;
                blueSpawnMax.yz = bounds.AABB.Max.yz;

                for (int i = 0; i < spawner.BlueBeeCount; ++i)
                {
                    float3 position = random.NextFloat3(bounds.AABB.Min, blueSpawnMax);

                    var instance = ecb.Instantiate(spawner.BlueBeePrefab);
                    var translation = new Translation { Value = position };
                    ecb.SetComponent(instance, translation);
                }

                float3 redSpawnMin = bounds.AABB.Max;
                redSpawnMin.x -= bounds.HiveOffset;
                redSpawnMin.yz = bounds.AABB.Min.yz;

                for (int i = 0; i < spawner.RedBeeCount; ++i)
                {
                    float3 position = random.NextFloat3(redSpawnMin, bounds.AABB.Max);

                    var instance = ecb.Instantiate(spawner.RedBeePrefab);
                    var translation = new Translation { Value = position };
                    ecb.SetComponent(instance, translation);
                }
            }).Run();

        ecb.Playback(EntityManager);
        ecb.Dispose();
    }
}