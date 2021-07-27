using Unity.Collections;
using Unity.Entities;
using Unity.Rendering;
using Unity.Transforms;
using Unity.Mathematics;

class SpawnerBeeSystem: SystemBase
{
    protected override void OnUpdate()
    {
        var ecb = new EntityCommandBuffer(Allocator.Temp);

        Entities
            .ForEach((Entity entity, in SpawnBeeConfig spawner) =>
            {
                ecb.DestroyEntity(entity);

                Random rng = new Random(123);
                for (int i = 0; i < spawner.BeeCount; ++i)
                {
                    var instance = ecb.Instantiate(spawner.BeePrefab);
                    float3 pos = rng.NextFloat3(spawner.SpawnLocation - spawner.SpawnAreaSize * 0.5f, spawner.SpawnLocation + spawner.SpawnAreaSize * 0.5f);
                    var translation = new Translation {Value = pos};
                    ecb.SetComponent(instance, translation);
                }
            }).Run();

        ecb.Playback(EntityManager);
        ecb.Dispose();
    }
}
