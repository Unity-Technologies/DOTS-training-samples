using Unity.Entities;
using Unity.Collections;
using Unity.Transforms;
using Unity.Mathematics;
using Unity.Rendering;
using UnityEngine;

public class SpawnerSystem : SystemBase
{
    public FireSim fireSimulation;
    public EntityQuery spawnerDeleted;

    protected override void OnCreate()
    {
        base.OnCreate();

        RequireForUpdate(spawnerDeleted);
    }

    protected override void OnStartRunning()
    {
        base.OnStartRunning();

        fireSimulation = GetSingleton<FireSim>();
    }

    protected override void OnUpdate()
    {
        var fireSim = fireSimulation;
        
        //World.GetExistingSystem<FirePropagationSystem>()

        //var seed = new System.Random();
        //var random = new Random((uint)seed.Next());
        var random = new Unity.Mathematics.Random(42);

        var ecb = new EntityCommandBuffer(Allocator.TempJob);

        var time = Time.ElapsedTime;

        Entities
            .WithStoreEntityQueryInField(ref spawnerDeleted)
            .ForEach((Entity entity, in Spawner spawner) =>
            {
                Entity instance;
                Translation translation;

                for (int i = 0; i < fireSim.FireGridDimension; ++i)
                {
                    for (int j = 0; j < fireSim.FireGridDimension; ++j)
                    {
                        instance = ecb.Instantiate(spawner.FireCell);
                        ecb.SetComponent(instance, new Translation { Value = new float3(i, -1.6f, j) });
                        ecb.AddComponent(instance, new FireCell { Temperature = 0 });
                        ecb.AddComponent(instance, new URPMaterialPropertyBaseColor { Value = new float4(0, 1, 0, 0) });
                    }
                }

                for (int j = 0; j < 4; ++j)
                {
                    for (int i = 0; i < fireSim.WaterCellCount; ++i)
                    {
                        instance = ecb.Instantiate(spawner.WaterCell);
                        switch (j)
                        {
                            case 0:
                                translation = new Translation { Value = new float3(i * 1.2f, 0.0f, -2.0f) };
                                break;
                            case 1:
                                translation = new Translation { Value = new float3(-2.0f, 0.0f, i * 1.2f) };
                                break;
                            case 2:
                                translation = new Translation { Value = new float3(i * 1.2f, 0.0f, fireSim.FireGridDimension + 2.0f) };
                                break;
                            default:
                                translation = new Translation { Value = new float3(fireSim.FireGridDimension + 2.0f, 0, i * 1.2f) };
                                break;
                        }
                        ecb.SetComponent(instance, translation);

                        ecb.AddComponent(instance, new WaterCell { Volume = random.NextInt(0, 100) });
                    }
                }

                for (int i = 0; i < fireSim.BucketCount; ++i)
                {
                    instance = ecb.Instantiate(spawner.BucketPrefab);
                    ecb.SetComponent(instance, new Translation { Value = new float3(random.NextInt(0, fireSim.FireGridDimension), 0.4f, random.NextInt(0, fireSim.FireGridDimension)) });
                    ecb.AddComponent<URPMaterialPropertyBaseColor>(instance);
                    ecb.AddComponent<BucketForScooper>(instance);
                    ecb.AddComponent<EmptyBucket>(instance);
                }

            }).Run();

        ecb.Playback(EntityManager);
        ecb.Dispose();

        ecb = new EntityCommandBuffer(Allocator.TempJob);

        Entities
            .ForEach((Entity entity, in Spawner spawner) =>
            {
                ecb.DestroyEntity(entity);

                for (int i = 0; i < fireSim.ChainCount; i++)
                {
                    var instance = ecb.CreateEntity();
                    var fullChain = ecb.AddBuffer<PasserFullBufferElement>(instance);
                    var emptyChain = ecb.AddBuffer<PasserEmptyBufferElement>(instance);

                    // Full chain
                    for (int j = 0; j < fireSim.NumBotsPerChain; j++)
                    {
                        instance = ecb.Instantiate(spawner.BotPrefab);
                        var botPosition = new float3(random.NextInt(0, fireSim.FireGridDimension), 1.6f, random.NextInt(0, fireSim.FireGridDimension));
                        ecb.SetComponent(instance, new Translation { Value = botPosition });
                        ecb.AddComponent(instance, new Bot { Index = j });
                        fullChain.Add(instance);

                        var color = spawner.PasserFullColor;
                        if (j == 0)
                        {
                            color = spawner.FillerColor;
                            ecb.AddComponent(instance, new FillerBot());
                        }
                        ecb.AddComponent(instance, new URPMaterialPropertyBaseColor() { Value = color });
                    }

                    // Empty chain
                    for (int j = 0; j < fireSim.NumBotsPerChain; j++)
                    {
                        instance = ecb.Instantiate(spawner.BotPrefab);
                        var botPosition = new float3(random.NextInt(0, fireSim.FireGridDimension), 1.6f, random.NextInt(0, fireSim.FireGridDimension));
                        ecb.SetComponent(instance, new Translation { Value = botPosition });
                        ecb.AddComponent(instance, new Bot { Index = fireSim.NumBotsPerChain + j });
                        emptyChain.Add(instance);

                        var color = spawner.PasserEmptyColor;
                        if (j == 0)
                        {
                            color = spawner.ThrowerColor;
                            ecb.AddComponent(instance, new ThrowerBot());
                        }
                        ecb.AddComponent(instance, new URPMaterialPropertyBaseColor() { Value = color });
                    }

                    // Scooper
                    instance = ecb.Instantiate(spawner.BotPrefab);
                    var scooperPosition = new float3(random.NextInt(0, fireSim.FireGridDimension), 1.6f, random.NextInt(0, fireSim.FireGridDimension));
                    ecb.SetComponent(instance, new Translation { Value = scooperPosition });
                    ecb.AddComponent(instance, new URPMaterialPropertyBaseColor() { Value = spawner.ScooperColor });
                    ecb.AddComponent(instance, new FindBucket());
                    ecb.AddComponent(instance, new ScooperBot() { ChainStart = fullChain[0] });
                }
            }).Run();

        ecb.Playback(EntityManager);
        ecb.Dispose();
    }
}