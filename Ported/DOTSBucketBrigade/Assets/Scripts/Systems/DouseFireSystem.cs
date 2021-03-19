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
            .ForEach((Entity entity, ref BucketID bucketId, in CurrentLine currentLine, in Translation position) =>
            {
                // Douse fire
                var fireIndex = (int)position.Value.x * gridSize + (int)position.Value.z;
                HeatMapDouseFireJob douseJob = new HeatMapDouseFireJob()
                {
                    FireIndex = (int) fireIndex,
                    GridSize = gridSize,
                    row = (int)position.Value.x,
                    col = (int)position.Value.z,
                    HeatMapBuffer = heatMap
                };
                douseJob.Schedule();

                SetComponent(bucketId.Value, new Volume(){ Value = 0.0f });
                
                // Drop bucket
                //ecb.AddComponent<Reposition>(currentLine.Value);
                ecb.RemoveComponent<CarryingBucket>(entity);

            }).Run();
    }
}


public struct HeatMapDouseFireJob : IJob
{
    public int FireIndex;
    public int GridSize;
    public int row;
    public int col;
    [NativeDisableContainerSafetyRestriction] public DynamicBuffer<HeatMap> HeatMapBuffer;

    public void Execute()
    {
        for (int r = row - 1; r < row + 1; r++)
        {
            for (int c = col - 1; c < col + 1; c++)
            {
                if (r >= 0 && r < GridSize && c >= 0 && c < GridSize)
                {
                    HeatMap heatMap = HeatMapBuffer[r * GridSize + c];
                    heatMap.Value = 0.0f;
                    HeatMapBuffer[r * GridSize + c] = heatMap;
                }
            }
        }
    }
}