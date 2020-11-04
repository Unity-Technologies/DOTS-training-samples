using Unity.Entities;
using Unity.Collections;
using Unity.Transforms;
using Unity.Mathematics;
using Unity.Rendering;

public class SpawnerSystem : SystemBase
{
    protected override void OnUpdate()
    {
        FireSim fireSim = GetSingleton<FireSim>();

        var time = Time.ElapsedTime;
        var ecb = new EntityCommandBuffer(Allocator.Temp);

        Entities
            .ForEach((Entity entity, in Spawner spawner) =>
            {
                ecb.DestroyEntity(entity);

                var instance;
                Translation translation;

                for (int i = 0; i < fireSim.FireGridDimension; ++i)
                {
                    for (int j = 0; j < fireSim.FireGridDimension; ++j)
                    {
                        instance = ecb.Instantiate(spawner.FireCell);
                        translation = new Translation { Value = new float3(i, -1.6f, j) };
                        ecb.SetComponent(instance, translation);

                        ecb.AddComponent(instance, new FireCell { Temperature = 0 });

                        ecb.AddComponent(instance, new URPMaterialPropertyBaseColor
                        {
                            Value = new float4(0, 1, 0, 0)
                        });
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

                        ecb.AddComponent(instance, new WaterCell { Volume = UnityEngine.Random.Range(0, 100) });
                    }
                }


                for (int i = 0; i < fireSim.BucketCount; ++i)
                {
                    instance = ecb.Instantiate(spawner.BucketPrefab);
                    translation = new Translation { Value = new float3(UnityEngine.Random.Range(0.0f, fireSim.FireGridDimension), 0.4f, UnityEngine.Random.Range(0.0f, fireSim.FireGridDimension)) };
                    ecb.SetComponent(instance, translation);
                }

                instance = ecb.Instantiate(spawner.ScooperPrefab);
                translation = new Translation { Value = new float3(0.0f, 0.0f, 0.0f) };
                ecb.SetComponent(instance, translation);
                ecb.SetComponent(instance, new MoveTowardBucket());

                F

            }).Run();

        ecb.Playback(EntityManager);
    }
}