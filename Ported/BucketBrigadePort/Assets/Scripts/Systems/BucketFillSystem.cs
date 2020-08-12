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
    }

    protected override void OnUpdate()
    {
        // hardcoded value for lake capacity 1000
        // hardcoded water amount for a bucketWater 1
        // hardcoded range of a lake 5
        var capacity    = 1000.0f;
        var bucketWater = 1.0f;
        var lakeRange   = 5.0f;

        var lakesTranslations =
            m_lakeQuery.ToComponentDataArray<Translation>(Allocator.TempJob);
        var bucketsTranslations =
            m_bucketQuery.ToComponentDataArray<Translation>(Allocator.TempJob);

        // input: position of the lake, RO, location where the lake should be
        //        WaterAmount of the lake, RW, decrease by the water amount of a bucket (hardcoded WATERAMOUNT)
        //        postion of the bucket, RO, used to calculate the distance to the lake, need to be within a hardcoded DISTANCE.
        //        WaterAmount of the bucket, 
        // output: waterAmount of the lake, 
        Entities
        .WithName("bucket_fill_lakes")
        .WithAll<WaterRefill>()
        .WithDisposeOnCompletion(bucketsTranslations)
        .ForEach((ref WaterAmount lakeWaterAmount, in Translation position) =>
        {
            if(lakeWaterAmount.Value < capacity)
            {
                for(int i = 0; i < bucketsTranslations.Length; i++)
                {
                    var dist = Vector3.Distance(position.Value, bucketsTranslations[i].Value);
                    if(dist <= lakeRange)
                    {
                        lakeWaterAmount.Value -= bucketWater;
                    }
                }
            }
        }).ScheduleParallel();

        // output: waterAmount of the bucket
        Entities
        .WithName("bucket_fill_buckets")
        .WithNone<WaterRefill>()
        .WithDisposeOnCompletion(lakesTranslations)
        .ForEach((ref WaterAmount bucketWaterAmount, in Translation position) =>
        {
            for (int i = 0; i < lakesTranslations.Length; i++)
            {
                var dist = Vector3.Distance(position.Value, lakesTranslations[i].Value);
                if (dist <= lakeRange)
                {
                    bucketWaterAmount.Value += bucketWater;
                }
            }

        }).ScheduleParallel();
    }
}
