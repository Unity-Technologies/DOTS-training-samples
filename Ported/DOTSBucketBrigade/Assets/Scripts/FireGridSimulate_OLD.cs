/*
using System.Linq;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

public class FireGridSimulate : SystemBase
{

    [BurstCompile]
    struct GridSimJob : IJobParallelFor
    {
        [ReadOnly] public NativeArray<FireGridCell> InputCells;
        
        public NativeArray<FireGridCell> OutputCells;

        public int2 Dimensions;
        public int SearchSize;
        public float Flashpoint;
        public float TransferRate;

        public void Execute(int index)
        {
            FireGridCell cell = InputCells[index];

            int cellRowIndex = index / Dimensions.x;
            int cellColumnIndex = index % Dimensions.x;

            float change = 0;

            for (int y = cellRowIndex - SearchSize; y <= cellRowIndex + SearchSize; y++)
            {
                if (y >= 0 && y < Dimensions.y)
                {
                    for (int x = cellColumnIndex - SearchSize; x <= cellColumnIndex + SearchSize; x++)
                    {
                        if (x >= 0 && x < Dimensions.x)
                        {
                            FireGridCell neighbourCell = InputCells[y * Dimensions.x + x];

                            if (neighbourCell.Temperature > Flashpoint)
                            {
                                change += neighbourCell.Temperature * TransferRate;
                            }
                        }
                    }
                }
            }

            cell.Temperature = math.clamp(cell.Temperature + change, -1.0f, 1.0f);
            OutputCells[index] = cell;
        }
    }

    [BurstCompile]
    struct GridCopyJob : IJob
    {
        public NativeArray<FireGridCell> OldCells;
        [DeallocateOnJobCompletion] public NativeArray<FireGridCell> NewCells;

        public void Execute()
        {
            OldCells.CopyFrom(NewCells);
        }
    }

    protected override void OnCreate()
    {
        RequireSingletonForUpdate<BucketBrigadeConfig>();
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
        var cells = EntityManager.GetBuffer<FireGridCell>(fireGrid).AsNativeArray();
        
        var newCells = new NativeArray<FireGridCell>(size, Allocator.TempJob, NativeArrayOptions.UninitializedMemory);

        Dependency = new GridSimJob
        {
            InputCells = cells,
            OutputCells = newCells,
            Dimensions = dimensions,
            SearchSize = searchSize,
            Flashpoint = flashpoint,
            TransferRate = transfer * delta
        }.Schedule(size, 1000, Dependency);

        Dependency = new GridCopyJob
        {
            OldCells = cells,
            NewCells = newCells
        }.Schedule(Dependency);
    }
}
*/