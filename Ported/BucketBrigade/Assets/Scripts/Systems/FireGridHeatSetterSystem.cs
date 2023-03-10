using System;
using Authoring;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;
using Random = Unity.Mathematics.Random;

namespace Systems
{
    [UpdateAfter(typeof(FireGridSpawnerSystem))]
    public partial struct FireGridHeatSetterSystem : ISystem
    {
        float timeUntilFireUpdate;
        
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<ConfigAuthoring.Config>();
            state.RequireForUpdate<ConfigAuthoring.FlameHeat>();
            timeUntilFireUpdate = 0;
        }
        
        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            timeUntilFireUpdate -= SystemAPI.Time.DeltaTime;
            
            var config = SystemAPI.GetSingleton<ConfigAuthoring.Config>();
            var heatMap = SystemAPI.GetSingletonBuffer<ConfigAuthoring.FlameHeat>();

            if (timeUntilFireUpdate < 0)
            {
                //can't use Allocator.Temp with a job
                var previousHeatMap = CollectionHelper.CreateNativeArray<ConfigAuthoring.FlameHeat>(heatMap.Length, state.WorldUpdateAllocator);
                previousHeatMap.CopyFrom(heatMap.AsNativeArray());

                var flamePropagationJob = new FlamePropagationJob
                {
                    readHeat = previousHeatMap,
                    writeHeat = heatMap.AsNativeArray(),
                    flashPoint = config.flashpoint,
                    heatRadius = config.heatRadius,
                    heatTransferRate = config.heatTransferRate,
                    numColumns = config.numColumns,
                    numRows = config.numRows
                };

                state.Dependency = flamePropagationJob.Schedule(heatMap.Length, 100, state.Dependency);
                timeUntilFireUpdate = config.fireSimUpdateRate;
            }

            Random random = new Random(123);
            var flameRenderJob = new FlameRenderJob 
                { heatMap = heatMap,
                    coolColor = config.fireCoolColor, 
                    flashPoint = config.flashpoint,
                    flickerRange = config.flickerRange,
                    flickerRate = config.flickerRate,
                    hotColor = config.fireHotColor,
                    maxFlameHeight = config.maxFlameHeight,
                    neutralColor = config.fireNeutralColor,
                    elapsedTime = (float)SystemAPI.Time.ElapsedTime,
                    random = random
                };
     
            state.Dependency = flameRenderJob.ScheduleParallel(state.Dependency);
        }
    }

    [BurstCompile]
    public struct FlamePropagationJob : IJobParallelFor
    {
        [ReadOnly] public NativeArray<ConfigAuthoring.FlameHeat> readHeat;
        public NativeArray<ConfigAuthoring.FlameHeat> writeHeat;
        public int numColumns;
        public int numRows;
        public int heatRadius;
        public float flashPoint;
        public float heatTransferRate;
        
        public void Execute(int index)
        {
            //update one cell  
            float tempChange = 0f;
            int cellRowIndex = Mathf.FloorToInt((float)index / numColumns);
            int cellColumnIndex = index % numColumns;
            
            for (int rowIndex = -heatRadius; rowIndex <= heatRadius; rowIndex++)
            {
                int currentRow = cellRowIndex - rowIndex;
                if (currentRow >= 0 && currentRow < numRows)
                {
                    for (int columnIndex = -heatRadius; columnIndex <= heatRadius; columnIndex++)
                    {
                        int currentColumn = cellColumnIndex + columnIndex;
                        if (currentColumn >= 0 && currentColumn < numColumns)
                        {
                            int neighbourIndex = currentRow * numColumns + currentColumn;
                            float neighbourHeat = readHeat[neighbourIndex].Value;
                            if (neighbourHeat > flashPoint)
                            {
                                tempChange += neighbourHeat * heatTransferRate;
                            }
                        }
                    }
                }
            }
            
            float newHeat = math.clamp(readHeat[index].Value + tempChange,-1f,1f);
            writeHeat[index] = new ConfigAuthoring.FlameHeat {Value = newHeat};
        }
    }

    [BurstCompile]
    [WithAll(typeof(FlameCell))]
    public partial struct FlameRenderJob : IJobEntity
    {
        [ReadOnly] public DynamicBuffer<ConfigAuthoring.FlameHeat> heatMap;
        public float flashPoint;
        public float maxFlameHeight;
        public float flickerRange;
        public float flickerRate;
        public float4 neutralColor;
        public float4 coolColor;
        public float4 hotColor;
        public float elapsedTime;
        public Unity.Mathematics.Random random;
        
        public void Execute(ref LocalTransform localTransform, ref URPMaterialPropertyBaseColor color, [EntityIndexInQuery] int index)
        {
            //setting colour based on heat
            var heat = heatMap[index];
  
            if (heat.Value < flashPoint)
            {
                localTransform.Position.y = -(maxFlameHeight * 0.5f) + random.NextFloat(0.01f,0.02f);
                color.Value = neutralColor;
            }
            else
            {
                float yPos = (-maxFlameHeight*0.5f + (heat.Value * maxFlameHeight)) - flickerRange;
                yPos += (flickerRange * 0.5f) + noise.cnoise(new float2((elapsedTime - index) * flickerRate - heat.Value, heat.Value)) * flickerRange;
                
                localTransform.Position.y = yPos;
                color.Value =
                    math.lerp(coolColor, hotColor, heat.Value);
            }
        }
    }
}


