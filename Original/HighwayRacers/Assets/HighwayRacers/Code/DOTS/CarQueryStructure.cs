using Unity.Collections;
using Unity.Mathematics;

public struct CarIndexAndState
{
    public int EntityArrayIndex;
    public CarBasicState State;
}

public partial struct CarQueryStructure
{
    public int CarCount;
    public float HighwayLen;

    // ReadOnly attributes are added here so that this struct can be copied to a job and accessed
    // in a read-only fashion.
    [ReadOnly] public NativeArray<CarIndexAndState> LaneCars;
    [ReadOnly] public NativeArray<int> LaneCarCounts;
    // Index into the sorted arrays (two at most for floor(lane) and ceil(lane)).
    [ReadOnly] public NativeArray<int2> OrderedCarLaneIndices;

    public CarQueryStructure(int carCount, float highwayLen)
    {
        CarCount = carCount;
        HighwayLen = highwayLen;
        LaneCars = new NativeArray<CarIndexAndState>(carCount * 4, Allocator.TempJob, NativeArrayOptions.UninitializedMemory);
        LaneCarCounts = new NativeArray<int>(4, Allocator.TempJob, NativeArrayOptions.ClearMemory);
        OrderedCarLaneIndices = new NativeArray<int2>(carCount, Allocator.TempJob, NativeArrayOptions.UninitializedMemory);
    }

    public void Dispose()
    {
        LaneCars.Dispose();
        LaneCarCounts.Dispose();
        OrderedCarLaneIndices.Dispose();
    }
}
