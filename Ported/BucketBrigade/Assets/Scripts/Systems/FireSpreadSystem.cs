using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

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
        
        if (!HasSingleton<TuningData>())
        {
            return;
        }

        var tuningData = GetSingleton<TuningData>();
        
        int numRows = tuningData.GridSize.x;
        int numCols = tuningData.GridSize.y;
        float selfHeatTick = tuningData.ValueIncreasePerTick;
        float neighborTick = tuningData.ValuePropagationPerTick;
        float spreadThreshold = tuningData.ValueThreshold;
        float maxValue = tuningData.MaxValue;

        float dt = Time.DeltaTime;
        
        NativeArray<float> startingVal = new NativeArray<float>(numRows * numCols,Allocator.TempJob);
        
        var gridPopulateJob = Entities.ForEach((in ValueComponent va, in GridIndex gridIndex) =>
        {
            startingVal[gridIndex.Index.y * numCols + gridIndex.Index.x] = va.Value;
            
        }).ScheduleParallel(Dependency);
        
        var gridUpdateJob = Entities
            .WithDeallocateOnJobCompletion(startingVal)
            .ForEach((ref ValueComponent fireValue, in GridIndex gridIndices) =>
        {
            bool validLeft = isValidIndex(startingVal.Length, gridIndices.Index.x - 1, gridIndices.Index.y,numRows);
            bool validRight = isValidIndex(startingVal.Length, gridIndices.Index.x + 1, gridIndices.Index.y,numRows);
            bool validUp = isValidIndex(startingVal.Length, gridIndices.Index.x, gridIndices.Index.y - 1,numRows);
            bool validDown = isValidIndex(startingVal.Length, gridIndices.Index.x, gridIndices.Index.y + 1,numRows);
            
            bool validUpLeft = isValidIndex(startingVal.Length, gridIndices.Index.x - 1, gridIndices.Index.y - 1,numRows);
            bool validUpRight = isValidIndex(startingVal.Length, gridIndices.Index.x + 1, gridIndices.Index.y - 1,numRows);
            bool validDownLeft = isValidIndex(startingVal.Length, gridIndices.Index.x - 1, gridIndices.Index.y + 1,numRows);
            bool validDownRight = isValidIndex(startingVal.Length, gridIndices.Index.x + 1, gridIndices.Index.y + 1,numRows);
            
            int flatIndexLeft = flatIndex(gridIndices.Index.x - 1, gridIndices.Index.y,numRows);
            int flatIndexRight = flatIndex(gridIndices.Index.x + 1, gridIndices.Index.y,numRows);
            int flatIndexUp = flatIndex(gridIndices.Index.x , gridIndices.Index.y - 1,numRows);
            int flatIndexDown = flatIndex(gridIndices.Index.x, gridIndices.Index.y + 1,numRows);
            
            int flatIndexUpLeft = flatIndex(gridIndices.Index.x - 1, gridIndices.Index.y - 1,numRows);
            int flatIndexUpRight = flatIndex(gridIndices.Index.x + 1, gridIndices.Index.y - 1,numRows);
            int flatIndexDownLeft = flatIndex(gridIndices.Index.x - 1, gridIndices.Index.y + 1,numRows);
            int flatIndexDownRight = flatIndex(gridIndices.Index.x + 1, gridIndices.Index.y + 1,numRows);

            //todo will just burn the entire forest without identifying what should selfHeat
            //fireValue.Value += selfHeatTick * dt;
            
            //"left" neighrbor
            if (validLeft && startingVal[flatIndexLeft] >= spreadThreshold)
            {
                fireValue.Value += neighborTick * dt * (startingVal[flatIndexLeft] / tuningData.MaxValue);
            }
            //"Right" neighrbor
            if (validRight && startingVal[flatIndexRight] >= spreadThreshold)
            {
                fireValue.Value += neighborTick * dt * (startingVal[flatIndexRight] / tuningData.MaxValue);
            }
            //"Up" neighrbor
            if (validUp && startingVal[flatIndexUp] >= spreadThreshold)
            {
                fireValue.Value += neighborTick * dt * (startingVal[flatIndexUp] / tuningData.MaxValue);
            }
            //"Down" neighrbor
            if (validDown && startingVal[flatIndexDown] >= spreadThreshold)
            {
                fireValue.Value += neighborTick * dt * (startingVal[flatIndexDown] / tuningData.MaxValue);
            }
            
            //"Upleft" neighrbor
            if (validUpLeft && startingVal[flatIndexUpLeft] >= spreadThreshold)
            {
                fireValue.Value += neighborTick * dt * (startingVal[flatIndexUpLeft] / tuningData.MaxValue);
            }
            //"UpRight" neighrbor
            if (validUpRight && startingVal[flatIndexUpRight] >= spreadThreshold)
            {
                fireValue.Value += neighborTick * dt * (startingVal[flatIndexUpRight] / tuningData.MaxValue);
            }
            //"DownLeft" neighrbor
            if (validDownLeft && startingVal[flatIndexDownLeft] >= spreadThreshold)
            {
                fireValue.Value += neighborTick * dt * (startingVal[flatIndexDownLeft] / tuningData.MaxValue);
            }
            //"DownRight" neighrbor
            if (validDownRight && startingVal[flatIndexDownRight] >= spreadThreshold)
            {
                fireValue.Value += neighborTick * dt * (startingVal[flatIndexDownRight] / tuningData.MaxValue);
            }
            
            var clamp = math.clamp(fireValue.Value, 0, maxValue);
            fireValue.Value = clamp;
            
        }).ScheduleParallel(gridPopulateJob);
        
        Dependency = gridUpdateJob;
    }
}
