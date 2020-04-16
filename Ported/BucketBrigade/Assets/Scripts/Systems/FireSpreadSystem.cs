using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

public class FireSpreadSystem: SystemBase
{


    static int flatIndex(int i, int j, int rows)
    {
      return j * rows + i;   
    }
    
    static bool isValidIndex(int length, int i, int j, int rows)
    {
        int index = flatIndex(i, j, rows);

        return index < length && index > 0;
    }
    
    protected override void OnUpdate()
    {
        float kDebugThreshold = 1f;
        float kDebugMaxThreshold = 2000f;

        //var tuningData = GetSingleton<TuningData>();
        
        int numRows = 10;
        int numCols = 10;
        float incr = 1;
        //float maxThreshold = tuningData.ValueIncreasePerTick;
        
        NativeArray<float> startingVal = new NativeArray<float>(numRows * numCols,Allocator.TempJob);
        
        var gridPopulateJob = Entities.ForEach((in ValueComponent va, in GridIndex gridIndex) =>
        {
            startingVal[gridIndex.Index.y * numCols + gridIndex.Index.x] = va.Value;
            
        }).ScheduleParallel(Dependency);
        
        var gridUpdateJob = Entities
            .WithoutBurst()
            .WithDeallocateOnJobCompletion(startingVal)
            .ForEach((ref ValueComponent fireValue, in GridIndex gridIndices) =>
        {
            bool validLeft = isValidIndex(startingVal.Length, gridIndices.Index.x - 1, gridIndices.Index.y,numRows);
            bool validRight = isValidIndex(startingVal.Length, gridIndices.Index.x + 1, gridIndices.Index.y,numRows);
            bool validUp = isValidIndex(startingVal.Length, gridIndices.Index.x, gridIndices.Index.y - 1,numRows);
            bool validDown = isValidIndex(startingVal.Length, gridIndices.Index.x, gridIndices.Index.y + 1,numRows);
            
            int flatIndexLeft = flatIndex(gridIndices.Index.x - 1, gridIndices.Index.y,numRows);
            int flatIndexRight = flatIndex(gridIndices.Index.x + 1, gridIndices.Index.y,numRows);
            int flatIndexUp = flatIndex(gridIndices.Index.x , gridIndices.Index.y - 1,numRows);
            int flatIndexDown = flatIndex(gridIndices.Index.x, gridIndices.Index.y + 1,numRows);
            
            
            
            //"left" neighrbor
            if (validLeft && startingVal[flatIndexLeft] >= kDebugThreshold)
            {
                fireValue.Value += incr;
            }
            //"Right" neighrbor
            if (validRight && startingVal[flatIndexRight] >= kDebugThreshold)
            {
                fireValue.Value += incr;
            }
            //"Up" neighrbor
            if (validUp && startingVal[flatIndexUp] >= kDebugThreshold)
            {
                fireValue.Value += incr;
            }
            //"Down" neighrbor
            if (validDown && startingVal[flatIndexDown] >= kDebugThreshold)
            {
                fireValue.Value += incr;
            }
            
            var clamp = math.clamp(fireValue.Value, 0, kDebugMaxThreshold);
            fireValue.Value = clamp;
            
        }).ScheduleParallel(gridPopulateJob);
        
        Dependency = gridUpdateJob;
    }
}
