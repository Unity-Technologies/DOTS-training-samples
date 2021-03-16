using System.Transactions;
using Unity.Entities;
using Unity.Collections;
using Unity.Mathematics;
using Unity.Transforms;

public class CarouselSpawnerSystem : SystemBase
{
    protected override void OnUpdate()
    {
        var ecb = new EntityCommandBuffer(Allocator.Temp);
        var outOfBoundsPosition = new float3(-10.0f, 0.0f, 0.0f);

            Entities
            .ForEach((Entity entity, in CarouselSpawner spawner) =>
            {
                // Destroying the current entity is a classic ECS pattern,
                // when something should only be processed once then forgotten.
                ecb.DestroyEntity(entity);
                var spawnCount = (int) (spawner.Distance * spawner.Frequency);
                for (var i = 0; i < spawnCount; ++i)
                {
                    var instance = ecb.Instantiate(spawner.SpawnPrefab);
                    ecb.SetComponent(instance, new Translation()
                    {
                        Value = outOfBoundsPosition
                    });
                }
            }).Run();

        ecb.Playback(EntityManager);
        ecb.Dispose();
    } 
}