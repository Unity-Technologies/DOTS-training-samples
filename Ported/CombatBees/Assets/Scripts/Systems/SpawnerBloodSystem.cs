using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;

class SpawnerBloodSystem : SystemBase
{
    protected override void OnUpdate()
    {
        var ecb = new EntityCommandBuffer(Allocator.Temp);
        var gameConfig = GetSingleton<GameConfig>();
        var spawnAreaSize = new float3(0.1f, 0.1f, 0.1f);

        Entities
            .ForEach((Entity entity, in SpawnBloodConfig spawner) =>
            {
                ecb.DestroyEntity(entity);

                Random rng = new Random(123);
                for (int i = 0; i < spawner.SplattersCount; ++i)
                {
                    var instance = ecb.Instantiate(gameConfig.BloodPrefab);
                    float3 pos = rng.NextFloat3(spawner.SpawnLocation - spawnAreaSize, spawner.SpawnLocation + spawnAreaSize);
                    var translation = new Translation { Value = pos };
                    ecb.SetComponent(instance, translation);
                    ecb.AddComponent(instance, new Blood
                    {
                        Speed = new float3(rng.NextFloat(), 0, rng.NextFloat())
                    });
                }
            }).Run();

        ecb.Playback(EntityManager);
        ecb.Dispose();
    }
}