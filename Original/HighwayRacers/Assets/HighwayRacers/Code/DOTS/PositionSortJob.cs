using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;

[BurstCompile]
public struct PositionSortJob : IJobParallelFor
{
    public int CarCount;
    [ReadOnly] public NativeArray<int> LaneCounts;

    [NativeDisableParallelForRestriction]
    public NativeArray<CarEntityAndState> Lanes;

    private struct PositionSort : IComparer<CarEntityAndState>
    {
        public int Compare(CarEntityAndState x, CarEntityAndState y)
            => x.State.Position.CompareTo(y.State.Position);
    }

    public void Execute(int index)
    {
        Lanes.Slice(index * CarCount, LaneCounts[index]).Sort(new PositionSort());
    }
}
