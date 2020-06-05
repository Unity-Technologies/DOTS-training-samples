/*
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;

public class FireGridSimulate : SystemBase
{
    

    protected override void OnCreate()
    {
        RequireSingletonForUpdate<BucketBrigadeConfig>();

        // Need this so that ECS realises we actually want to write to this
        EntityQuery query = GetEntityQuery(ComponentType.ReadWrite<FireGridCell>());
        base.OnCreate();
        
    }

    // TODO: Rolling sum
    //[BurstCompile]
    struct GridAccumulateAxisJob : IJob
    {
        [DeallocateOnJobCompletion] [ReadOnly] public NativeArray<float> InputCells;
        
        [NativeDisableParallelForRestriction] public NativeArray<float> OutputCells;
        [DeallocateOnJobCompletion] public NativeArray<float> AccumBuffer;

        public int2 Dimensions;
        public int SearchSize;
        public float Flashpoint;
        public bool CheckFlashPoint;

        public void Execute()
        {
            //float[] size = new float[10];
            int accumBufferSize = (SearchSize * 2) + 1;
            //float[] accumBuffer = new float[accumBufferSize];
            int accumBufferIdx = 0;
            int currentCellBufferIndex = 0;
            float accumulatedTotal = 0;
            
            for (int index = 0; index < InputCells.Length; ++index)
            {
                int cellRowIndex = index / Dimensions.x;
                int cellColumnIndex = index % Dimensions.x;

                if (cellColumnIndex == 0)
                {
                    accumulatedTotal = 0;
                    currentCellBufferIndex = SearchSize;
                    accumBufferIdx = currentCellBufferIndex;
                    for (int i = 0; i < accumBufferSize; ++i)
                    {
                        AccumBuffer[i] = 0;
                    }

                    for (int fwdOffset = 0; fwdOffset < SearchSize; ++fwdOffset)
                    {
                        float value = InputCells[index + fwdOffset];
                        AccumBuffer[accumBufferIdx++] = value;

                        if (!CheckFlashPoint || value > Flashpoint)
                            accumulatedTotal += value;
                    }
                }
                //[][][][][]

                // Remove Old Value if needed
                if (!CheckFlashPoint || AccumBuffer[accumBufferIdx] > Flashpoint)
                {
                    accumulatedTotal -= AccumBuffer[accumBufferIdx]; // remove tail
                }
                // Get New Value
                AccumBuffer[accumBufferIdx] = index + SearchSize < Dimensions.x ? InputCells[index + SearchSize] : 0;
                // Update Total
                if (!CheckFlashPoint || AccumBuffer[accumBufferIdx] > Flashpoint)
                {
                    accumulatedTotal += AccumBuffer[accumBufferIdx];
                }
                // Increment Index
                accumBufferIdx = (++accumBufferIdx) % accumBufferSize;

                // calculate target index for output
                int newIndex = cellColumnIndex * Dimensions.x + cellRowIndex;
                // get current value of the InputCells
                OutputCells[newIndex] = accumulatedTotal;

            }

            accumulatedTotal += 5;
        }

    }

//    [BurstCompile]
    struct GridFinaliseJob : IJobParallelFor
    {
        [DeallocateOnJobCompletion] [ReadOnly] public NativeArray<float> InputCells;
        
        [NativeDisableParallelForRestriction] public NativeArray<FireGridCell> OutputCells;

        public float TransferRate;
        public float Flashpoint;

        public void Execute(int index)
        {
            FireGridCell cell = OutputCells[index];

            float value = InputCells[index] - (cell.Temperature > Flashpoint ? cell.Temperature : 0);

            cell.Temperature = math.clamp(cell.Temperature + (value * TransferRate), -1.0f, 1.0f);
            OutputCells[index] = cell;
        }
    }

    protected override void OnUpdate()
    {
        var config = GetSingleton<BucketBrigadeConfig>();
        int2 dimensions = config.GridDimensions;
        float transfer = config.TemperatureIncreaseRate;
        float flashpoint = config.Flashpoint;
        int searchSize = config.HeatRadius;
        int size = dimensions.x * dimensions.y;

        float delta = Time.DeltaTime;

        var fireGrid = GetSingletonEntity<FireGrid>();
        var cellBufferAsArray = EntityManager.GetBuffer<FireGridCell>(fireGrid).AsNativeArray();
        
        var gridCopy = new NativeArray<float>(size, Allocator.TempJob, NativeArrayOptions.UninitializedMemory);
        gridCopy.CopyFrom(NativeArrayExtensions.Reinterpret<FireGridCell, float>(cellBufferAsArray));
        
        var accumColumns = new NativeArray<float>(size, Allocator.TempJob, NativeArrayOptions.UninitializedMemory);
        var accumRows = new NativeArray<float>(size, Allocator.TempJob, NativeArrayOptions.UninitializedMemory);
        
       

        // Accumulate columns
        Dependency = new GridAccumulateAxisJob
        {
            InputCells = gridCopy,
            OutputCells = accumColumns,
            AccumBuffer = new NativeArray<float>(searchSize * 2 +1, Allocator.TempJob),
            Dimensions = dimensions,
            SearchSize = searchSize,
            Flashpoint = flashpoint,
            CheckFlashPoint = true,
        }.Schedule(Dependency);
        
        // Accumulate rows
        Dependency = new GridAccumulateAxisJob
        {
            InputCells = accumColumns,
            OutputCells = accumRows,
            AccumBuffer = new NativeArray<float>(searchSize * 2 + 1, Allocator.TempJob),
            Dimensions = dimensions,
            SearchSize = searchSize,
            Flashpoint = flashpoint,
            CheckFlashPoint = false,
        }.Schedule(Dependency);

        Dependency = new GridFinaliseJob
        {
            InputCells = accumRows,
            OutputCells = cellBufferAsArray,
            Flashpoint = flashpoint,
            TransferRate = transfer * delta
        }.Schedule(size, 1000, Dependency);
    }
}
//*/