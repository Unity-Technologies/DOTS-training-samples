
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;

public class FireGridSimulate : SystemBase
{
    EndSimulationEntityCommandBufferSystem m_EndSimulationEcbSystem;
    
    // TODO: Rolling sum
    [BurstCompile]
    struct GridAccumulateAxisJob : IJobParallelFor
    {
        [DeallocateOnJobCompletion] [ReadOnly] public NativeArray<float> InputCells;
        
        [NativeDisableParallelForRestriction] public NativeArray<float> OutputCells;

        public int2 Dimensions;
        public int SearchSize;
        public float Flashpoint;
        public bool CheckFlashPoint;

        public void Execute(int index)
        {
            int cellRowIndex = index / Dimensions.x;
            int cellColumnIndex = index % Dimensions.x;

            float change = 0;

            int y = cellRowIndex;

            for (int offset = -SearchSize; offset <= SearchSize; offset++)
            {
                if (offset != 0)
                {
                    int x = cellColumnIndex + offset;
                    if (x >= 0 && x < Dimensions.x)
                    {
                        float neighbourCellTemperature = InputCells[y * Dimensions.x + x];

                        if (!CheckFlashPoint || neighbourCellTemperature > Flashpoint)
                        {
                            change += neighbourCellTemperature;
                        }
                    }
                }
            }

            int newIndex = cellColumnIndex * Dimensions.x + cellRowIndex;
            OutputCells[newIndex] = change;
        }
    }

    [BurstCompile]
    struct GridFinaliseJob : IJobParallelFor
    {
        [DeallocateOnJobCompletion] [ReadOnly] public NativeArray<float> InputCells;
        
        [NativeDisableParallelForRestriction] public NativeArray<FireGridCell> OutputCells;

        public float TransferRate;

        public void Execute(int index)
        {
            FireGridCell cell = OutputCells[index]; 
            cell.Temperature = math.clamp(cell.Temperature + (InputCells[index] * TransferRate), -1.0f, 1.0f);
            OutputCells[index] = cell;
        }
    }

    protected override void OnCreate()
    {
        RequireSingletonForUpdate<BucketBrigadeConfig>();
        
        m_EndSimulationEcbSystem = World
            .GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
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

            Dimensions = dimensions,
            SearchSize = searchSize,
            Flashpoint = flashpoint,
            CheckFlashPoint = true,
        }.Schedule(size, 1000, Dependency);
        
        // Accumulate rows
        Dependency = new GridAccumulateAxisJob
        {
            InputCells = accumColumns,
            OutputCells = accumRows,

            Dimensions = dimensions,
            SearchSize = searchSize,
            Flashpoint = flashpoint,
            CheckFlashPoint = false,
        }.Schedule(size, 1000, Dependency);

        Dependency = new GridFinaliseJob
        {
            InputCells = accumRows,
            OutputCells = cellBufferAsArray,
            
            TransferRate = transfer * delta
        }.Schedule(size, 1000, Dependency);
        
        // Make sure that the ECB system knows about our job
        m_EndSimulationEcbSystem.AddJobHandleForProducer(Dependency);
    }
}