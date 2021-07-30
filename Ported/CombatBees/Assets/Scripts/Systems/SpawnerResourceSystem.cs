using Unity.Collections;
using Unity.Entities;
using Unity.Rendering;
using Unity.Transforms;
using Unity.Mathematics;

[UpdateInGroup(typeof(InitializationSystemGroup))]
class SpawnerResourceSystem: SystemBase
{
    protected override void OnUpdate()
    {
        var ecb = new EntityCommandBuffer(Allocator.Temp);
        var gameConfig = GetSingleton<GameConfig>();

        Entities
            .ForEach((Entity entity, in SpawnResourceConfig spawner) =>
            {
                ecb.DestroyEntity(entity);

                Random rng = new Random(123);
                for (int i = 0; i < spawner.ResourceCount; ++i)
                {
                    var instance = ecb.Instantiate(gameConfig.ResourcePrefab);
                    float3 pos = rng.NextFloat3(spawner.SpawnLocation - spawner.SpawnAreaSize * 0.5f, spawner.SpawnLocation + spawner.SpawnAreaSize * 0.5f);
                    var translation = new Translation {Value = pos};
                    ecb.SetComponent(instance, translation);
                    ecb.SetComponent(instance, new Resource
                    {
                        Speed = 0.0f,
                        CarryingBee = Entity.Null
                    });
                    ecb.SetComponent(instance, new NewTranslation {translation = translation});
                }
            }).Run();

        ecb.Playback(EntityManager);
        ecb.Dispose();
    }
}
