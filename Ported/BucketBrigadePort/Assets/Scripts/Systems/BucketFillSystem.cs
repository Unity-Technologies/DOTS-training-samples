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
    }

    protected override void OnUpdate()
    {
        // hardcode settings
        var bucketWater     = 1.0f;
        var lakeDistRange   = 5.0f;

        var bucketFullColor = GetSingleton<BucketColorSettings>().Full;

        var lakeEntities = m_lakeQuery.ToEntityArray(Allocator.TempJob);

        var waterAmountComponents = GetComponentDataFromEntity<WaterAmount>(false);
        var colorComponents = GetComponentDataFromEntity<Color>(false);

        // For each tosser that has a empty bucket
        Entities
        .WithName("bucket_fill_filler")
        .WithAll<BotRoleFiller>()
        .WithNone<TargetPosition>()
        .WithReadOnly(lakeEntities)
        .WithNativeDisableContainerSafetyRestriction(waterAmountComponents)
        .WithNativeDisableContainerSafetyRestriction(colorComponents)
        .WithDisposeOnCompletion(lakeEntities)
        .ForEach((in BucketRef bucketRef, in Translation position) =>
        {
            for (int i = 0; i < lakeEntities.Length; i++)
            {
                var lakeWaterAmount = GetComponent<WaterAmount>(lakeEntities[i]);
                var lakeTranslation = GetComponent<Translation>(lakeEntities[i]);

                // not enough water in the lake
                if (lakeWaterAmount.Value < bucketWater)
                {
                    continue;
                }

                var fillerLakeDist = Vector3.Distance(position.Value, lakeTranslation.Value);

                // bucketFiller is not in the lake
                if (fillerLakeDist > lakeDistRange)
                {
                    break;
                }

                // Reduce the water amount in the lake
                //lakeWaterAmountComponents[i] = new WaterAmount { Value = lakeWaterAmounts[i].Value - bucketWater };
                waterAmountComponents[lakeEntities[i]] = new WaterAmount { Value = lakeWaterAmount.Value - bucketWater };
                waterAmountComponents[bucketRef.Value] = new WaterAmount { Value = bucketWater };
                colorComponents[bucketRef.Value] = new Color { Value = bucketFullColor };
            }

        }).ScheduleParallel();
    }
}
