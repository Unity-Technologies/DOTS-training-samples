using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[BurstCompile]
struct FireSimJob : IJobParallelFor
{
    public int   grid;
    public int   rad;
    public float threshold;
    public float falloff;

    [WriteOnly]
    public NativeArray<HeatMapElement> heatMap;
    [ReadOnly]
    public NativeArray<HeatMapElement> copyOfHeatMap;

    public void Execute(int i)
    {
        var col = i % grid;
        var row = i / grid;

        var heatContribution = 0f;

        var minRow = math.max(row - rad, 0);
        var maxRow = math.min(row + rad, grid - 1);
        var minCol = math.max(col - rad, 0);
        var maxCol = math.min(col + rad, grid - 1);
        for (var j = minRow; j <= maxRow; j++)
        {
            for (var k = minCol; k <= maxCol; k++)
            {
                var index = k + j * grid;
                var heat = copyOfHeatMap[index].temperature;
                if (index != i)
                {
                    if (heat >= threshold)
                        heatContribution += heat * falloff;
                }
                else
                    heatContribution += heat;
            }
        }

        heatMap[i] = new HeatMapElement(){ temperature = math.min(heatContribution,1)};
    }
}

public class FireSimSystem : SystemBase
{
    protected override void OnCreate()
    {
        RequireSingletonForUpdate<GameConfigComponent>();
        RequireSingletonForUpdate<HeatMapElement>();
    }
    
    protected override void OnUpdate()
    {
        var config = GetSingleton<GameConfigComponent>();

        var heatMapEntity = GetSingletonEntity<HeatMapElement>();
        var heatMapBuffer = GetBuffer<HeatMapElement>(heatMapEntity);
        var copyOfHeatMap = heatMapBuffer.ToNativeArray(Allocator.TempJob);

        var job = new FireSimJob() {
            grid = config.SimulationSize,
            rad = config.HeatTrasferRadius,
            threshold = config.FlashPoint,
            falloff = config.HeatFallOff,
            heatMap = heatMapBuffer.AsNativeArray(),
            copyOfHeatMap = copyOfHeatMap
        };

        Dependency = job.Schedule(heatMapBuffer.Length, 128, Dependency);
        Dependency = copyOfHeatMap.Dispose(Dependency);
    }
}
