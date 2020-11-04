using Unity.Entities;
using Unity.Collections;
using Unity.Transforms;
using Unity.Mathematics;
using Unity.Rendering;

public class SpawnerSystem : SystemBase
{
    protected override void OnUpdate()
    {
        //World.GetExistingSystem<FirePropagationSystem>()
        FireSim fireSim = GetSingleton<FireSim>();

        var time = Time.ElapsedTime;
        var random = new Random(42);
        var ecb = new EntityCommandBuffer(Allocator.TempJob);
        var em = EntityManager;

        Entities
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
                    translation = new Translation { Value = new float3(random.NextInt(0, fireSim.FireGridDimension), 0.4f, random.NextInt(0, fireSim.FireGridDimension)) };
                    ecb.SetComponent(instance, translation);
                    ecb.AddComponent(instance, new EmptyBucket());
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
                    var instance = ecb.Instantiate(spawner.ScooperPrefab);
                    var scooperPosition = new float3(random.NextInt(0, fireSim.FireGridDimension), 0.0f, random.NextInt(0, fireSim.FireGridDimension));
                    ecb.SetComponent(instance, new Translation { Value = scooperPosition });
                    ecb.AddComponent(instance, new MoveTowardBucket() { Target = FireSimSystem.GetClosestBucket(scooperPosition, em) });
                }
            }).Run();

        ecb.Playback(EntityManager);
        ecb.Dispose();
    }
}