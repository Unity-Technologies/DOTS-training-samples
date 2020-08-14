using System.Diagnostics.Tracing;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using Unity.Transforms;

public class BucketTossSystem : SystemBase
{
    protected override void OnUpdate()
    {
        var bucketEmptyColor = GetSingleton<BucketColorSettings>().Empty;
        var fireFlashPoint = GetSingleton<FireSpreadSettings>().flashpoint;
        var coolStrength = GetSingleton<FireSpreadSettings>().coolStrength;
        var coolRadious = GetSingleton<FireSpreadSettings>().coolRadious;

        var fireSpreadSettings = GetSingleton<FireSpreadSettings>();
        var tileSpawner = GetSingleton<TileSpawner>();
        int columns = tileSpawner.XSize;
        int rows = tileSpawner.YSize;

        int tileCount = rows * columns;
        NativeArray<float> fireCellTemperatures = new NativeArray<float>(tileCount, Allocator.TempJob);
        NativeArray<float3> fireCellTranslations = new NativeArray<float3>(tileCount, Allocator.TempJob);

        // Get all of the temperatures in the expected spatial order.
        Entities
        .WithName("bucket_toss_firecell_reading")
        .ForEach((in Temperature temperature, in Tile tile, in Translation translation) =>
        {
            fireCellTemperatures[tile.Id] = temperature.Value;
            fireCellTranslations[tile.Id] = translation.Value;
        }).ScheduleParallel();

        var waterAmountComponents = GetComponentDataFromEntity<WaterAmount>(false);
        var colorComponents = GetComponentDataFromEntity<Color>(false);

        // Handle fileCell being tossed at
        // Input: tosser -  entities that have BotRoleTosser tag, BucketRef (Entity reference), do not have TargetPosition, 
        //                  
        Entities
        .WithName("bucket_toss_tosser_standingby")
        .WithAll<BotRoleTosser>()
        .WithNone<TargetPosition>()
        .WithDisposeOnCompletion(fireCellTranslations)
        .WithReadOnly(fireCellTranslations)
        .WithNativeDisableContainerSafetyRestriction(waterAmountComponents)
        .WithNativeDisableContainerSafetyRestriction(colorComponents)
        .ForEach((ref BucketRef bucketRef, in Translation position) =>
        {
            for (int i = 0; i < tileCount; i++)
            {
                var tosserFireDist = math.distance(position.Value, fireCellTranslations[i]);

                // tosser is not close to the firecell
                if (tosserFireDist > coolRadious)
                {
                    continue;
                }

                fireCellTemperatures[i] -= coolStrength;
            }

            waterAmountComponents[bucketRef.Value] = new WaterAmount { Value = 0 };
            colorComponents[bucketRef.Value] = new Color { Value = bucketEmptyColor };

        }).ScheduleParallel();

        // Apply the temporary array
        Entities
        .WithName("bucket_toss_copyback_firecell")
        .WithDisposeOnCompletion(fireCellTemperatures)
        .WithReadOnly(fireCellTemperatures)
        .ForEach((ref Temperature temperature, in Tile tile) =>
        {
            temperature.Value = fireCellTemperatures[tile.Id];
        }).ScheduleParallel();        
    }
}