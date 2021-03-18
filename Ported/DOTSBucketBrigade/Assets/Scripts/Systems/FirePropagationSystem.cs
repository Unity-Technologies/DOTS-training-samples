using System.Collections.Generic;
using System.Linq;
using System.Runtime;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;
using Random = UnityEngine.Random;

public class FirePropagationSystem : SystemBase
{
    static public readonly float FireSimUpdateRate = 0.1f;
    static public readonly float HeatTransferRate = 0.003f;
    static public readonly int HeatRadius = 1;
    static public readonly float flashPoint = 0.2f;

    private double _timeUntilFireUpdate;
    
    protected override void OnCreate()
    {
        _timeUntilFireUpdate = FireSimUpdateRate;
        RequireSingletonForUpdate<InitCounts>();
    }

    protected override void OnUpdate()
    {
        _timeUntilFireUpdate -= Time.DeltaTime;
        if (_timeUntilFireUpdate <= 0.0)
        {
            _timeUntilFireUpdate = FireSimUpdateRate;

            int gridSize = GetSingleton<InitCounts>().GridSize;
            JobHandle combinedDependencies = default;
            JobHandle dep = Dependency;
            
            Entities
                .ForEach((ref DynamicBuffer<HeatMap> heatMapBuffer) =>
                {
                    NativeArray<HeatMap> heatMapBufferArray = heatMapBuffer.ToNativeArray(Allocator.TempJob);
                    int cpuCount = SystemInfo.processorCount;
                    int segLength = heatMapBuffer.Length / cpuCount;
                    if (heatMapBuffer.Length % cpuCount > 0)
                    {
                        ++segLength;
                    }
                    
                    for (int i = 0; i < cpuCount; ++i)
                    {
                        HeatMapPropagationJob propagateJob = new HeatMapPropagationJob()
                        {
                            StartIndex = segLength * i,
                            EndIndex = math.min(segLength * i + segLength - 1, heatMapBuffer.Length - 1),
                            GridSize = gridSize,
                            HeatMapBufferArray = heatMapBufferArray,
                            HeatMapBuffer = heatMapBuffer   
                        };           
                        
                        combinedDependencies = JobHandle.CombineDependencies(propagateJob.Schedule(dep), combinedDependencies);
                    }

                    heatMapBufferArray.Dispose(combinedDependencies);
 
                }).Run();

            Dependency = combinedDependencies;
        }
    }
}

public struct HeatMapPropagationJob : IJob
{
    public int StartIndex;
    public int EndIndex;
    public int GridSize;
    [ReadOnly] public NativeArray<HeatMap> HeatMapBufferArray;
    [NativeDisableContainerSafetyRestriction] public DynamicBuffer<HeatMap> HeatMapBuffer;

    public void Execute()
    {
        for (int cellIndex = StartIndex; cellIndex <= EndIndex; ++cellIndex)
        {
            float tempChange = 0.0f;
            int cellRowIndex = cellIndex / GridSize;
            int cellColIndex = cellIndex % GridSize;

            for (int rowIndex = -FirePropagationSystem.HeatRadius;
                rowIndex <= FirePropagationSystem.HeatRadius;
                rowIndex++)
            {
                int currentRow = cellRowIndex + rowIndex;
                if (currentRow >= 0 && currentRow < GridSize)
                {
                    for (int columnIndex = -FirePropagationSystem.HeatRadius;
                        columnIndex <= FirePropagationSystem.HeatRadius;
                        columnIndex++)
                    {
                        if (rowIndex == 0 && columnIndex == 0)
                        {
                            continue;
                        }
                        
                        int currentColumn = cellColIndex + columnIndex;
                        if (currentColumn >= 0 && currentColumn < GridSize)
                        {
                            int neighbourIndex = (currentRow * GridSize) + currentColumn;
                            ;
                            if (HeatMapBufferArray[neighbourIndex].Value > FirePropagationSystem.flashPoint)
                            {
                                tempChange += HeatMapBufferArray[neighbourIndex].Value *
                                              FirePropagationSystem.HeatTransferRate;
                            }
                        }
                    }
                }
            }

            HeatMap heatMap = HeatMapBufferArray[cellIndex];
            heatMap.Value = math.min(1.0f, heatMap.Value + tempChange);
            HeatMapBuffer[cellIndex] = heatMap;
        }
    }
}


