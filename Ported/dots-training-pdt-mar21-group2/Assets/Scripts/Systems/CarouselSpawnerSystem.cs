using System.Transactions;
using Unity.Entities;
using Unity.Collections;
using Unity.Mathematics;
using Unity.Transforms;

public class CarouselSpawnerSystem : SystemBase
{
    protected override void OnUpdate()
    {
        var worldBounds = GetSingleton<WorldBounds>();
        var ecb = new EntityCommandBuffer(Allocator.Temp);
        var outOfBoundsPosition = new float3(-10.0f, 0.0f, 0.0f);

        Entities
            .ForEach((Entity entity, in CarouselSpawner spawner) =>
            {
                // Destroying the current entity is a classic ECS pattern,
                // when something should only be processed once then forgotten.
                ecb.DestroyEntity(entity);
                var spawnCount = (int) (worldBounds.Width * spawner.Frequency);
                for (var i = 0; i < spawnCount; ++i)
                {
                    var instance = ecb.Instantiate(spawner.SpawnPrefab);
                    ecb.SetComponent(instance, new Translation()
                    {
                        Value = outOfBoundsPosition
                    });
                    ecb.AddComponent(instance, new Available());
                    if (HasComponent<Rock>(spawner.SpawnPrefab))
                    {
                        ecb.AddComponent<Scale>(instance, new Scale {Value = 1.0f});
                    }
                    // TODO(sandy): random scale
                    // TODO(sandy): add support for target scale for growing during spawn
                }
            }).Run();

        ecb.Playback(EntityManager);
        ecb.Dispose();
    }
}