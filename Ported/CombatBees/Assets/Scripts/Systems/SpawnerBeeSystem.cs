using Unity.Collections;
using Unity.Entities;
using Unity.Rendering;
using Unity.Transforms;
using Unity.Mathematics;

[UpdateInGroup(typeof(InitializationSystemGroup))]
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
                    if(spawner.Team == 0)
                        ecb.AddComponent<TeamA>(instance);
                    else
                        ecb.AddComponent<TeamB>(instance);
                    ecb.AddComponent(instance, new NewTranslation {translation = translation});
                    ecb.AddComponent(instance, new Bee
                    {
                        State = BeeState.Idle,
                        Target = Entity.Null,
                        resource = Entity.Null
                    });
                }
            }).Run();

        ecb.Playback(EntityManager);
        ecb.Dispose();
    }
}
