using Unity.Collections;
using Unity.Mathematics;

public struct CarEntityAndState
{
    public int EntityIndex;
    public CarBasicState State;
}

public partial struct CarQueryStructure
{
    public int CarCount;

    // ReadOnly attributes are added here so that this struct can be copied to a job and accessed
    // in a read-only fashion.
    [ReadOnly] public NativeArray<CarEntityAndState> CarLanes;
    [ReadOnly] public NativeArray<int> LaneCarCounts;
    // Index into the sorted arrays (two at most for floor(lane) and ceil(lane)).
    [ReadOnly] public NativeArray<int2> OrderedCarLaneIndices;

    public CarQueryStructure(int carCount)
    {
        CarCount = carCount;
        CarLanes = new NativeArray<CarEntityAndState>(carCount * 4, Allocator.TempJob, NativeArrayOptions.UninitializedMemory);
        LaneCarCounts = new NativeArray<int>(4, Allocator.TempJob, NativeArrayOptions.ClearMemory);
        OrderedCarLaneIndices = new NativeArray<int2>(carCount, Allocator.TempJob, NativeArrayOptions.UninitializedMemory);
    }

    public void Dispose()
    {
        CarLanes.Dispose();
        LaneCarCounts.Dispose();
        OrderedCarLaneIndices.Dispose();
    }
}
