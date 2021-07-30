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
        RequireSingletonForUpdate<GameConfig>();
        
    }

    protected override void OnUpdate()
    {
        var ecb = new EntityCommandBuffer(Allocator.Temp);
        var leftColor = GetSingleton<ShaderOverrideLeftColor>().Value;
        var rightColor = GetSingleton<ShaderOverrideRightColor>().Value;
        var gameConfig = GetSingleton<GameConfig>();

        Entities
            .ForEach((Entity entity, in SpawnBeeConfig spawner) =>
            {
                ecb.DestroyEntity(entity);
                Random rng = new Random(123);
                for (int i = 0; i < spawner.BeeCount; ++i)
                {
                    var instance = ecb.Instantiate(spawner.Team == 0 ? gameConfig.BeePrefabA : gameConfig.BeePrefabB);
                    float3 pos = rng.NextFloat3(spawner.SpawnLocation - spawner.SpawnAreaSize * 0.5f, spawner.SpawnLocation + spawner.SpawnAreaSize * 0.5f);
                    var translation = new Translation {Value = pos};
                    ecb.SetComponent(instance, translation);

                    const bool useNewShader = true;

                    if (spawner.Team == 0)
                    {
                        ecb.AddComponent<TeamA>(instance);

                        if (useNewShader)
                        {
                            ecb.SetComponent(instance, new BeeShaderOverrideColor
                            {
                                Value = leftColor
                            });
                        }
                        else
                        {
                            ecb.SetComponent(instance,new URPMaterialPropertyBaseColor
                            {
                                Value = leftColor
                            });
                        }
                    }
                    else
                    {
                        ecb.AddComponent<TeamB>(instance);
                        if (useNewShader)
                        {

                            ecb.SetComponent(instance, new BeeShaderOverrideColor
                            {
                                Value = rightColor
                            });
                        }
                        else
                        {
                            ecb.SetComponent(instance, new URPMaterialPropertyBaseColor
                            {
                                Value = rightColor
                            });
                        }
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
