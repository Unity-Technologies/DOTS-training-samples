using System.Diagnostics.Tracing;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using Unity.Transforms;

public class BucketFillSystem : SystemBase
{
    private EntityQuery m_lakeQuery;
    private EntityQuery m_bucketQuery;
    private EntityQuery m_bucketFillerQuery;

    protected override void OnCreate()
    {
        m_lakeQuery = GetEntityQuery(new EntityQueryDesc
        {
            All = new[]
            {
                ComponentType.ReadOnly<Translation>(),
                typeof(WaterAmount),
                ComponentType.ReadOnly<WaterRefill>()
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

        m_bucketFillerQuery = GetEntityQuery(new EntityQueryDesc
        {
            All = new[]
            {
                ComponentType.ReadOnly<Translation>(),
                ComponentType.ReadOnly<Bot>(),
                ComponentType.ReadOnly<BotRoleFiller>()
            }
        });
    }

    protected override void OnUpdate()
    {
        // hardcode settings
        var bucketWater     = 1.0f;
        var lakeDistRange   = 5.0f;
        var bucketDistRange = 1.0f;

        var bucketFullColor = GetSingleton<BucketColorSettings>().Full;

        var lakeTranslations =
            m_lakeQuery.ToComponentDataArray<Translation>(Allocator.TempJob);
        var lakeWaterAmounts =
            m_lakeQuery.ToComponentDataArray<WaterAmount>(Allocator.TempJob);
        var bucketTranslations =
            m_bucketQuery.ToComponentDataArray<Translation>(Allocator.TempJob);
        var bucketWaterAmounts =
            m_bucketQuery.ToComponentDataArray<WaterAmount>(Allocator.TempJob);
        var bucketFillerTranslations =
            m_bucketFillerQuery.ToComponentDataArray<Translation>(Allocator.TempJob);

        // input:   lake - position RO, waterAmount RW
        //          bucketFiller - position RO
        //          bucket - position RO, waterAmount RW
        // output:  lake -  waterAmount, decrease by one bucket waterAmount if there is a bucketFiller in the lake and
        //                  the bucketFiller has an emtpy bucket.
        Entities
        .WithName("bucket_fill_lakes")
        .WithAll<WaterRefill>()
        .WithDisposeOnCompletion(bucketTranslations)
        .WithDisposeOnCompletion(bucketWaterAmounts)
        .ForEach((ref WaterAmount lakeWaterAmount, in Translation position) =>
        {
            // There is a bucketFiller in the lake
            for (int i = 0; i < bucketFillerTranslations.Length; i++)
            {
                // Lake has at lease one bucket of water
                if (lakeWaterAmount.Value < bucketWater)
                {
                    break;
                }

                var fillerDist = Vector3.Distance(position.Value, bucketFillerTranslations[i].Value);
                
                // bucketFiller is not in the lake
                if (fillerDist > lakeDistRange)
                {
                    continue;
                }

                // The bucketFiller has an empty bucket
                for (int j = 0; j < bucketTranslations.Length; j++)
                {
                    // Lake has at lease one bucket of water
                    if (lakeWaterAmount.Value < bucketWater)
                    {
                        break;
                    }

                    // The bucket is not empty
                    if (bucketWaterAmounts[j].Value > 0)
                    {
                        continue;
                    }
                        
                    var bucketDist = Vector3.Distance(bucketFillerTranslations[i].Value, bucketTranslations[j].Value);

                    // The empty bucket is not with the bucketFiller
                    if (bucketDist > bucketDistRange)
                    {
                        continue;
                    }
                        
                    // Decrease lakeWaterAmount by one bucket
                    lakeWaterAmount.Value -= bucketWater;
                    if (lakeWaterAmount.Value < 0)
                    {
                        lakeWaterAmount.Value = 0;
                    }
                }
            }
        }).ScheduleParallel();

        // output: waterAmount of the bucket
        Entities
        .WithName("bucket_fill_buckets")
        .WithAll<WaterAmount>()
        .WithNone<WaterRefill>()
        .WithDisposeOnCompletion(lakeTranslations)
        .WithDisposeOnCompletion(lakeWaterAmounts)
        .WithDisposeOnCompletion(bucketFillerTranslations)
        .ForEach((ref WaterAmount bucketWaterAmount, ref Color bucketColor, in Translation position) =>
        {
            // There is a bucketFiller in the lake
            for (int i = 0; i < lakeTranslations.Length; i++)
            {
                // Bucket is not empty
                if (bucketWaterAmount.Value >= bucketWater)
                {
                    break;
                }

                // Lake has at lease one bucket of water
                if (lakeWaterAmounts[i].Value < bucketWater)
                {
                    break;
                }

                // The bucketFiller has an empty bucket
                for (int j = 0; j < bucketFillerTranslations.Length; j++)
                {
                    var fillerDist = Vector3.Distance(lakeTranslations[i].Value, bucketFillerTranslations[j].Value);

                    // bucketFiller is not in the lake
                    if (fillerDist > lakeDistRange)
                    {
                        continue;
                    }

                    var bucketDist = Vector3.Distance(position.Value, bucketFillerTranslations[j].Value);

                    // The empty bucket is not with the bucketFiller
                    if (bucketDist > bucketDistRange)
                    {
                        continue;
                    }

                    // fill the bucket
                    bucketWaterAmount.Value += bucketWater;
                    bucketColor.Value = bucketFullColor;
                    if (bucketWaterAmount.Value > bucketWater)
                    {
                        bucketWaterAmount.Value = bucketWater;
                    }
                    break;
                }
            }

        }).ScheduleParallel();
    }
}
