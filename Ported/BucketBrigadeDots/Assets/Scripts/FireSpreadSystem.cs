using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

public partial struct FireSpreadSystem : ISystem
{
    private float timeUntilFireUpdate;
    
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<GameSettings>();
        state.RequireForUpdate<FireTemperature>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var deltaTime = SystemAPI.Time.DeltaTime;
        timeUntilFireUpdate -= deltaTime;
        if (timeUntilFireUpdate > 0f) return;
        
        var settings = SystemAPI.GetSingleton<GameSettings>();
        timeUntilFireUpdate += settings.FireSimUpdateRate;
        var size = settings.Size;
        var cols = settings.RowsAndColumns;
        
        var temperatures = SystemAPI.GetSingletonBuffer<FireTemperature>().Reinterpret<float>();
        var writeBuffer = new NativeArray<float>(size, Allocator.TempJob);

        var spreadJob = new FireSpreadJob
        {
            cols = cols,
            heatRadius = settings.HeatRadius,
            heatTransferRate = settings.HeatTransferRate,
            temperatures = temperatures,
            writeBuffer = writeBuffer
        };
        var jobCount = (int)math.ceil((float)size / SystemInfo.processorCount);
        var spreadHandle = spreadJob.Schedule(size, jobCount, state.Dependency);

        var writeJob = new WriteBufferToTemperatures
        {
            temperatures = temperatures,
            writeBuffer = writeBuffer
        };
        
        
        state.Dependency = writeJob.Schedule(spreadHandle);
        writeBuffer.Dispose(state.Dependency);

        // for (var i = 0; i < size; i++)
        // {
        //     var increase = 0f;
        //
        //     var currentX = i % cols;
        //     var currentY = i / cols;
        //
        //     for (var offsetY = -settings.HeatRadius; offsetY <= settings.HeatRadius; offsetY++)
        //     {
        //         var neighborY = currentY + offsetY;
        //         if (neighborY < 0 || neighborY >= cols) continue;
        //         
        //         for (var offsetX = -settings.HeatRadius; offsetX <= settings.HeatRadius; offsetX++)
        //         {
        //             var neighborX = currentX + offsetX;
        //             if (neighborX < 0 || neighborX >= cols) continue;
        //
        //             var neighborIndex = neighborY * cols + neighborX;
        //             var neighborHeat = temperatures[neighborIndex];
        //             if (neighborHeat < .2f) continue;
        //             
        //             increase += neighborHeat * settings.HeatTransferRate;
        //         }
        //     }
        //
        //     writeBuffer[i] = math.min(1f, temperatures[i] + increase);
        // }

        // for (var i = 0; i < size; i++)
        // {
        //     temperatures[i] = writeBuffer[i];
        // }
    }

    [BurstCompile]
    struct FireSpreadJob : IJobParallelFor
    {
        public int cols;
        public int heatRadius;
        public float heatTransferRate;

        [ReadOnly] public DynamicBuffer<float> temperatures;
        public NativeArray<float> writeBuffer;

        public void Execute(int i)
        {
            var increase = 0f;

            var currentX = i % cols;
            var currentY = i / cols;

            for (var offsetY = -heatRadius; offsetY <= heatRadius; offsetY++)
            {
                var neighborY = currentY + offsetY;
                if (neighborY < 0 || neighborY >= cols) continue;
            
                for (var offsetX = -heatRadius; offsetX <= heatRadius; offsetX++)
                {
                    var neighborX = currentX + offsetX;
                    if (neighborX < 0 || neighborX >= cols) continue;

                    var neighborIndex = neighborY * cols + neighborX;
                    var neighborHeat = temperatures[neighborIndex];
                    if (neighborHeat < .2f) continue;
                
                    increase += neighborHeat * heatTransferRate;
                }
            }

            writeBuffer[i] = math.min(1f, temperatures[i] + increase);
        }
    }
    
    [BurstCompile]
    struct WriteBufferToTemperatures : IJob
    {
        public DynamicBuffer<float> temperatures;
        [ReadOnly]public NativeArray<float> writeBuffer;
        
        public void Execute()
        {
            temperatures.CopyFrom(writeBuffer);
        }
        
        // public void Execute(int index)
        // {
        //     temperatures[index] = writeBuffer[index];
        // }
    }
}
