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

                for (int i = 0; i < fireSim.FireGridDimension; ++i)
                {
                    for (int j = 0; j < fireSim.FireGridDimension; ++j)
                    {
                        var instance = ecb.Instantiate(spawner.FireCell);
                        var translation = new Translation { Value = new float3(i, -0.4f, j) };
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
                                translation = new Translation { Value = new float3(i * 1.2f, 0, fireSim.FireGridDimension + 2.0f) };
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
                    var instance = ecb.Instantiate(spawner.BucketPrefab);
                    var translation = new Translation { Value = new float3(UnityEngine.Random.Range(0.0f, fireSim.FireGridDimension), 0.5f, UnityEngine.Random.Range(0.0f, fireSim.FireGridDimension)) };
                    ecb.SetComponent(instance, translation);
                }

            }).Run();

        ecb.Playback(EntityManager);
    }
}