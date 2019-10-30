using System.Threading;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using Unity.Mathematics;

// Sort cars into a list of each lane. Cars might be in two lanes if it's changing lanes.

public struct CarEntityAndState
{
    public int EntityIndex;
    public CarBasicState State;
}

[BurstCompile]
public struct LaneSortJob : IJobForEachWithEntity<CarBasicState>
{
    public int CarCount;
    [NativeDisableContainerSafetyRestriction] public NativeArray<CarEntityAndState> Lanes;
    public NativeArray<int> LaneCounts;

    public unsafe void Execute(Entity entity, int index, [ReadOnly] ref CarBasicState carState)
    {
        UnityEngine.Debug.Assert(carState.Lane >= 0 && carState.Lane <= 3);
        var lane1 = (int)math.floor(carState.Lane);
        var lane2 = (int)math.ceil(carState.Lane);

        int* laneCountsRaw = (int*)LaneCounts.GetUnsafePtr();

        Lanes[lane1 * CarCount + Interlocked.Increment(ref laneCountsRaw[lane1]) - 1] = new CarEntityAndState()
        {
            EntityIndex = index,
            State = carState
        };
        if (lane2 != lane1)
        {
            Lanes[lane2 * CarCount + Interlocked.Increment(ref laneCountsRaw[lane2]) - 1] = new CarEntityAndState()
            {
                EntityIndex = index,
                State = carState
            };
        }
    }
}
