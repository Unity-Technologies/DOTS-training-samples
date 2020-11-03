using Unity.Entities;
using Unity.Collections;
using Unity.Transforms;
using Unity.Mathematics;
using UnityEngine;
using Unity.Rendering;

public class SpawnerSystem : SystemBase
{
    protected override void OnUpdate()
    {
        var ecb = new EntityCommandBuffer(Allocator.Temp);

        Entities
            .ForEach((Entity entity, in Spawner spawner) =>
            {
                ecb.DestroyEntity(entity);

                for (int i = 0; i < spawner.FireGridDimension; ++i)
                {
                    for (int j = 0; j < spawner.FireGridDimension; ++j)
                    {
                        var instance = ecb.Instantiate(spawner.FireCell);
                        var translation = new Translation { Value = new float3(i, -0.4f, j) };
                        ecb.SetComponent(instance, translation);

                        ecb.AddComponent(instance, new FireCell { Temperature = 0.0f });

                        ecb.AddComponent(instance, new URPMaterialPropertyBaseColor
                        {
                            Value = new float4(0, 1, 0, 0)
                        });
                    }
                }


                for (int j = 0; j < 4; ++j)
                    for (int i = 0; i < spawner.WaterCellCount; ++i)
                    {
                        var instance = ecb.Instantiate(spawner.WaterCell);
                        Translation translation;
                        switch (j)
                        {
                            case 0:
                                translation = new Translation { Value = new float3(i * 1.2f, 0, -2.0f) };
                                break;
                            case 1:
                                translation = new Translation { Value = new float3(-2.0f, 0, i * 1.2f) };
                                break;
                            case 2:
                                translation = new Translation { Value = new float3(i * 1.2f, 0, spawner.FireGridDimension + 2.0f) };
                                break;
                            default:
                                translation = new Translation { Value = new float3(spawner.FireGridDimension + 2.0f, 0, i * 1.2f) };
                                break;
                        }
                        ecb.SetComponent(instance, translation);

                        ecb.AddComponent(instance, new WaterCell { Volume = UnityEngine.Random.Range(0, 100) });
                    }


                for (int i = 0; i < spawner.BucketCount; ++i)
                {
                    var instance = ecb.Instantiate(spawner.BucketPrefab);
                    var translation = new Translation { Value = new float3(UnityEngine.Random.Range(0.0f, spawner.FireGridDimension), 0.5f, UnityEngine.Random.Range(0.0f, spawner.FireGridDimension)) };
                    ecb.SetComponent(instance, translation);
                }

            }).Run();

        ecb.Playback(EntityManager);
    }
}