using Unity.Collections;
using Unity.Entities;
using Unity.Rendering;
using Unity.Transforms;
using Unity.Mathematics;

[UpdateInGroup(typeof(InitializationSystemGroup))]
class SpawnerBeeSystem: SystemBase
{
    protected override void OnCreate()
    {
        RequireSingletonForUpdate<ShaderOverrideLeftColor>();
        RequireSingletonForUpdate<ShaderOverrideRightColor>();
    }

    protected override void OnUpdate()
    {
        var ecb = new EntityCommandBuffer(Allocator.Temp);
        var leftColor = GetSingleton<ShaderOverrideLeftColor>().Value;
        var rightColor = GetSingleton<ShaderOverrideRightColor>().Value;

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

                    if (spawner.Team == 0)
                    {
                        ecb.AddComponent<TeamA>(instance);
                        ecb.SetComponent(instance,new URPMaterialPropertyBaseColor
                                        {
                                            Value = leftColor
                                        });
                    }
                    else
                    {
                        ecb.AddComponent<TeamB>(instance);
                        ecb.SetComponent(instance, new URPMaterialPropertyBaseColor
                        {
                            Value = rightColor
                        });
                    }

                    ecb.SetComponent(instance, new NewTranslation {translation = translation});
                    ecb.SetComponent(instance, new Bee
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
