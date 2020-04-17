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
        NativeArray<float> waterVal = new NativeArray<float>(numRows * numCols,Allocator.TempJob);
        
        var gridPopulateJob = Entities.ForEach((ref WaterSplashData water, in ValueComponent va, in GridIndex gridIndex) =>
        {
            startingVal[gridIndex.Index.y * numCols + gridIndex.Index.x] = va.Value;
            waterVal[gridIndex.Index.y * numCols + gridIndex.Index.x] = water.Value;
            water.Value = 0;
            
        }).ScheduleParallel(Dependency);
        
        var gridUpdateJob = Entities
            .WithDeallocateOnJobCompletion(startingVal)
            .WithDeallocateOnJobCompletion(waterVal)
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
            if (validLeft)
            {
                if( startingVal[flatIndexLeft] >= spreadThreshold)
                    fireValue.Value += neighborTick * dt * (startingVal[flatIndexLeft] / tuningData.MaxValue);
                if (waterVal[flatIndexLeft] >= spreadThreshold)
                {
                    fireValue.Value -= (waterVal[flatIndexLeft]) * tuningData.WaterSplashFalloff;
                }
            }
            //"Right" neighrbor
            if (validRight)
            {
                if(startingVal[flatIndexRight] >= spreadThreshold)
                    fireValue.Value += neighborTick * dt * (startingVal[flatIndexRight] / tuningData.MaxValue);
                if (waterVal[flatIndexRight] >= spreadThreshold)
                {
                    fireValue.Value -= (waterVal[flatIndexRight]) * tuningData.WaterSplashFalloff;
                }
            }
            //"Up" neighrbor
            if (validUp)
            {
                if (startingVal[flatIndexUp] >= spreadThreshold)
                    fireValue.Value += neighborTick * dt * (startingVal[flatIndexUp] / tuningData.MaxValue);
                if (waterVal[flatIndexUp] >= spreadThreshold)
                {
                    fireValue.Value -= (waterVal[flatIndexUp]) * tuningData.WaterSplashFalloff;
                }
            }
            //"Down" neighrbor
            if (validDown)
            {
                if (startingVal[flatIndexDown] >= spreadThreshold)
                    fireValue.Value += neighborTick * dt * (startingVal[flatIndexDown] / tuningData.MaxValue);
                if (waterVal[flatIndexDown] >= spreadThreshold)
                {
                    fireValue.Value -= (waterVal[flatIndexDown]) * tuningData.WaterSplashFalloff;
                }
            }
            
            //"Upleft" neighrbor
            if (validUpLeft)
            {
                if (startingVal[flatIndexUpLeft] >= spreadThreshold)
                    fireValue.Value += neighborTick * dt * (startingVal[flatIndexUpLeft] / tuningData.MaxValue) * tuningData.ValueDiagonalFalloff;
                if(waterVal[flatIndexUpLeft] >= spreadThreshold)
                    fireValue.Value -= (waterVal[flatIndexUpLeft]) * tuningData.WaterSplashFalloff * tuningData.WaterSplashFalloff;
            }
            //"UpRight" neighrbor
            if (validUpRight)
            {
                if (startingVal[flatIndexUpRight] >= spreadThreshold)
                    fireValue.Value += neighborTick * dt * (startingVal[flatIndexUpRight] / tuningData.MaxValue) * tuningData.ValueDiagonalFalloff;
                if(waterVal[flatIndexUpRight] >= spreadThreshold)
                    fireValue.Value -= (waterVal[flatIndexUpRight]) * tuningData.WaterSplashFalloff * tuningData.WaterSplashFalloff;
            }
            //"DownLeft" neighrbor
            if (validDownLeft)
            {
                if (startingVal[flatIndexDownLeft] >= spreadThreshold)
                    fireValue.Value += neighborTick * dt * (startingVal[flatIndexDownLeft] / tuningData.MaxValue) * tuningData.ValueDiagonalFalloff;
                if(waterVal[flatIndexDownLeft] >= spreadThreshold)
                    fireValue.Value -= (waterVal[flatIndexDownLeft]) * tuningData.WaterSplashFalloff * tuningData.WaterSplashFalloff;
            }
            //"DownRight" neighrbor
            if (validDownRight)
            {
                if (startingVal[flatIndexDownRight] >= spreadThreshold)
                    fireValue.Value += neighborTick * dt * (startingVal[flatIndexDownRight] / tuningData.MaxValue) * tuningData.ValueDiagonalFalloff;
                if(waterVal[flatIndexDownRight] >= spreadThreshold)
                    fireValue.Value -= (waterVal[flatIndexDownRight]) * tuningData.WaterSplashFalloff * tuningData.WaterSplashFalloff;
            }
            
            var clamp = math.clamp(fireValue.Value, 0, maxValue);
            fireValue.Value = clamp;
            
        }).ScheduleParallel(gridPopulateJob);
        
        Dependency = gridUpdateJob;
    }
}
