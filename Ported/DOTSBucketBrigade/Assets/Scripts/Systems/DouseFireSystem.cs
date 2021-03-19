using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.UIElements;

public class DouseFireSystem : SystemBase
{
    private EntityQuery heatMapQuery;

    protected override void OnCreate()
    {
        RequireSingletonForUpdate<HeatMapTag>();
        RequireSingletonForUpdate<InitCounts>();
        heatMapQuery = GetEntityQuery(
            typeof(Cell),
            typeof(Translation)
        );
    }

    protected override void OnUpdate()
    {
        var ecb = new EntityCommandBuffer(Allocator.Temp);
        Entity heatMapEntity = GetSingletonEntity<HeatMapTag>();
        DynamicBuffer<HeatMap> heatMap = GetBuffer<HeatMap>(heatMapEntity);
        int gridSize = GetSingleton<InitCounts>().GridSize;

        Entities
            .WithAll<FullBucketer>()
            .WithAll<LastInLine>()
            .WithAll<CarryingBucket>()
            .ForEach((Entity entity, ref BucketID bucketId, in Translation position) =>
            {
                // Douse fire
                var fireIndex = (int) position.Value.z * gridSize + (int) position.Value.x;
                var heatMapValue = heatMap[fireIndex];
                heatMapValue.Value = 0.0f;
                heatMap[fireIndex] = heatMapValue;
                SetComponent(bucketId.Value, new Volume() {Value = 0.0f});

                // Drop bucket
                ecb.RemoveComponent<CarryingBucket>(entity);

            }).Run();
    }
}