using System;
using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;

[BurstCompile]
public struct TrainLookup : IEquatable<TrainLookup>
{
    public int LineID;
    public bool OnPlatformA;

    [BurstCompile]
    public bool Equals(TrainLookup other)
    {
        return LineID == other.LineID && OnPlatformA == other.OnPlatformA;
    }

    [BurstCompile]
    public override bool Equals(object obj)
    {
        return obj is TrainLookup other && Equals(other);
    }

    [BurstCompile]
    public override int GetHashCode()
    {
        return LineID * (OnPlatformA ? 1 : -1);
    }
}


[BurstCompile]
public struct TrainComponents
{
    public RefRO<Train> Train;
    public RefRO<LocalTransform> Transform;
    public Entity TrainEntity;
    public bool IsUnloading;
}
