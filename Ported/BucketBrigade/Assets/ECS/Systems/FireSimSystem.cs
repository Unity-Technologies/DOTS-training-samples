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
    public int grid;
    public int rad;
    public float threshold;
    public float falloff;
    public NativeArray<HeatMapElement> heatMap;
    [ReadOnly]
    public NativeArray<HeatMapElement> copyOfHeatMap;
    public void Execute(int i)
    {
        int col = i % grid;
        int row = i / grid;

        var heatContribution = 0f;

        for (int j = math.max(row - rad, 0); j <= math.min(row + rad, grid-1); j++)
        {
            for (int k = math.max(col - rad, 0); k <= math.min(col + rad, grid-1); k++)
            {
                var index = k + j * grid;
                var heat = copyOfHeatMap[index].temperature;
                if (index != i)
                {
                    if (heat >= threshold)
                    {
                        heatContribution += heat * falloff;
                    }
                }
                else
                {
                    heatContribution += heat;
                }
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
        var rad = config.HeatTrasferRadius;
        var falloff = config.HeatFallOff;
        var grid = config.SimulationSize;
        var threshold = config.FlashPoint;

        var heatMapEntity = GetSingletonEntity<HeatMapElement>();
        var heatMapBuffer = GetBuffer<HeatMapElement>(heatMapEntity);
        var heatMapArray = heatMapBuffer.AsNativeArray();
        var copyOfHeatMap = heatMapBuffer.ToNativeArray(Allocator.TempJob);

        var job = new FireSimJob() {
            grid = grid,
            rad = rad, 
            threshold = threshold, 
            falloff = falloff,
            heatMap = heatMapArray,
            copyOfHeatMap = copyOfHeatMap 
        };

        Dependency = job.Schedule(heatMapBuffer.Length, 128, Dependency);
        Dependency = copyOfHeatMap.Dispose(Dependency);

        return;
        
        Entities
            .ForEach((DynamicBuffer<HeatMapElement> heatMap) => {
            for (int i = 0; i < heatMap.Length; i++)
            {
                int col = i % grid;
                int row = i / grid;

                var heatContribution = 0f;

                for (int j = math.max(row - rad, 0); j <= math.min(row + rad, grid-1); j++)
                {
                    for (int k = math.max(col - rad, 0); k <= math.min(col + rad, grid-1); k++)
                    {
                        var index = k + j * grid;
                        var heat = heatMap[index].temperature;
                        if (index != i)
                        {
                            
                            if (heat >= threshold)
                            {
                                heatContribution += heat * falloff;
                                
                            }
                            
                        }
                        else
                        {
                            heatContribution += heat;
                        }
                        
                    }
                }

                heatMap[i] = new HeatMapElement(){ temperature = math.min(heatContribution,1)};
                
            }
        }).ScheduleParallel();
    }
}
