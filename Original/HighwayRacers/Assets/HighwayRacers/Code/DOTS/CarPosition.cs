using System;
using Unity.Entities;

[Serializable]
public struct CarBasicState : IComponentData
{
    public float Lane; // [0..4) //?
    public float Position; // Position on the road.
    public float Speed;
}

[Serializable]
public struct CarMergeSpace : IComponentData
{
    public float MergeSpace;
}
