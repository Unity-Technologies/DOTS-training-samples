using System.Diagnostics.Tracing;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using Unity.Transforms;

public class BucketTossSystem : SystemBase
{
    private EntityQuery m_fireCellQuery;
    private EntityQuery m_tosserQuery;
    private EntityQuery m_bucketQuery;

    protected override void OnCreate()
    {
        m_fireCellQuery = GetEntityQuery(new EntityQueryDesc
        {
            All = new[]
            {
                ComponentType.ReadOnly<Tile>(),
                typeof(Temperature)
            }
        });

        m_tosserQuery = GetEntityQuery(new EntityQueryDesc
        {
            All = new[]
            {
                ComponentType.ReadOnly<Translation>(),
                ComponentType.ReadOnly<BotRoleTosser>()
            }
        });

        m_bucketQuery = GetEntityQuery(new EntityQueryDesc
        {
            All = new[]
            {
                ComponentType.ReadOnly<Translation>(),
                ComponentType.ReadOnly<WaterAmount>()
            },

            None = new[]
            {
                ComponentType.ReadOnly<WaterRefill>()
            }
        });
    }

    protected override void OnUpdate()
    {
        // hardcode settings
        var bucketWater = 1.0f;
        var fireRange = 1.0f;
        var bucketDistRange = 1.0f;
        
        var bucketEmptyColor = GetSingleton<BucketColorSettings>().Empty;
        var fireFlashPoint = GetSingleton<FireSpreadSettings>().flashpoint;
        var coolStrength = GetSingleton<FireSpreadSettings>().coolStrength;

        var fireCellTranslations =
            m_fireCellQuery.ToComponentDataArray<Translation>(Allocator.TempJob);
        var fireCellTemperatures =
            m_fireCellQuery.ToComponentDataArray<Temperature>(Allocator.TempJob);
        var tosserTranslations =
            m_tosserQuery.ToComponentDataArray<Translation>(Allocator.TempJob);
        var bucketTranslations =
            m_bucketQuery.ToComponentDataArray<Translation>(Allocator.TempJob);
        var bucketWaterAmounts =
            m_bucketQuery.ToComponentDataArray<WaterAmount>(Allocator.TempJob);

        /*
        var tileSpawner = GetSingleton<TileSpawner>();
        int columns = tileSpawner.XSize;
        int rows = tileSpawner.YSize;
        int tileCount = rows * columns;
        NativeArray<float> temperatureArray = new NativeArray<float>(tileCount, Allocator.TempJob);
        */

        /*
        // Get all of the temperatures in the expected spatial order.
        Entities
        .WithName("bucket_toss_fireCell_read")
        .ForEach((in Temperature temperature, in Tile tile) =>
        {
            temperatureArray[tile.Id] = temperature.Value;
        }).ScheduleParallel();
        */

        // Handle fileCell being tossed at
        Entities
        .WithName("bucket_toss_fileCell")
        .WithAll<Temperature>()
        .WithDisposeOnCompletion(bucketTranslations)
        .WithDisposeOnCompletion(bucketWaterAmounts)
        .ForEach((ref Temperature fireCellTemperature, in Translation position, in Tile tile) =>
        {
            // fireCell is flashed
            if (fireCellTemperature.Value >= fireFlashPoint)
            {
                for (int i = 0; i < tosserTranslations.Length; i++)
                {
                    var tosserFireDist = Vector3.Distance(position.Value, tosserTranslations[i].Value);

                    // tosser is not close to the firecell
                    if (tosserFireDist > fireRange)
                    {
                        continue;
                    }

                    for (int j = 0; j < bucketTranslations.Length; j++)
                    {
                        // bucket is not full
                        if (bucketWaterAmounts[j].Value < bucketWater)
                        {
                            continue;
                        }

                        var tosserBucketDist = Vector3.Distance(bucketTranslations[j].Value, tosserTranslations[i].Value);

                        // full bucket is not with the tosser
                        if (tosserBucketDist > bucketDistRange)
                        {
                            continue;
                        }

                        fireCellTemperature.Value -= coolStrength * bucketWaterAmounts[j].Value;

                    }
                }
            }        
        }).ScheduleParallel();
    
        // Handle the bucket being tossed
        Entities
        .WithName("bucket_toss_buckets")
        .WithAll<WaterAmount>()
        .WithNone<WaterRefill>()
        .WithDisposeOnCompletion(fireCellTranslations)
        .WithDisposeOnCompletion(fireCellTemperatures)
        .WithDisposeOnCompletion(tosserTranslations)
        .ForEach((ref WaterAmount bucketWaterAmount, ref Color bucketColor, in Translation position) =>
        {
            for (int i = 0; i < tosserTranslations.Length; i++)
            {
                // bucket is not full
                if (bucketWaterAmount.Value < bucketWater)
                {
                    break;
                }

                var tosserBucketDist = Vector3.Distance(position.Value, tosserTranslations[i].Value);

                // full bucket is not with the tosser
                if (tosserBucketDist > bucketDistRange)
                {
                    continue;
                }

                for (int j = 0; j < fireCellTranslations.Length; j++)
                {
                    // fireCell not flashed
                    if (fireCellTemperatures[j].Value < fireFlashPoint)
                    {
                        continue;
                    }

                    var tosserFireDist = Vector3.Distance(tosserTranslations[i].Value, fireCellTranslations[j].Value);

                    // tosser is not close to a firecell
                    if (tosserFireDist > fireRange)
                    {
                        continue;
                    }

                    // Toss the bucket
                    bucketWaterAmount.Value -= bucketWater;
                    if (bucketWaterAmount.Value < 0)
                    {
                        bucketWaterAmount.Value = 0;
                    }
                    bucketColor.Value = bucketEmptyColor;
                }
            }

        }).ScheduleParallel();
    }
}