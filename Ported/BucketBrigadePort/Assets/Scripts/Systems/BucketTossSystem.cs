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
                ComponentType.ReadOnly<Translation>(),
                typeof(Temperature)
            }
        });

        m_tosserQuery = GetEntityQuery(new EntityQueryDesc
        {
            All = new[]
            {
                ComponentType.ReadOnly<Translation>(),
                ComponentType.ReadOnly<BotRoleTosser>(),
            }
        });        
    }

    protected override void OnUpdate()
    {
        // hardcoded water amount for a bucketWater 1
        // hardcoded range of a lake 5
        var bucketWater = 1.0f;
        var fireRange = 1.0f;

        var bucketEmptyColor = GetSingleton<BucketColorSettings>().Empty;

        var fireCellTranslations =
            m_fireCellQuery.ToComponentDataArray<Translation>(Allocator.TempJob);
        var tosserTranslations =
            m_tosserQuery.ToComponentDataArray<Translation>(Allocator.TempJob);
        
        Entities
        .WithName("bucket_toss_buckets")
        .WithAll<WaterAmount>()
        .WithNone<WaterRefill>()
        .WithDisposeOnCompletion(fireCellTranslations)
        .WithDisposeOnCompletion(tosserTranslations)
        .ForEach((ref WaterAmount bucketWaterAmount, ref Color bucketColor, in Translation position) =>
        {
            if (bucketWaterAmount.Value > 0)
            {
                for (int i = 0; i < fireCellTranslations.Length; i++)
                {
                    for (int j = 0; j < tosserTranslations.Length; j++)
                    {
                        var tosserFireDist = Vector3.Distance(fireCellTranslations[i].Value, tosserTranslations[j].Value);
                        if (tosserFireDist <= fireRange)
                        {
                            var tosserBucketDist = Vector3.Distance(position.Value, tosserTranslations[j].Value);

                            if (tosserFireDist <= fireRange)
                            {
                                bucketWaterAmount.Value -= bucketWater;
                                if(bucketWaterAmount.Value < 0)
                                {
                                    bucketWaterAmount.Value = 0;
                                }
                                bucketColor.Value = bucketEmptyColor;
                            }
                        }
                    }
                }
            }

        }).ScheduleParallel();
    }
}