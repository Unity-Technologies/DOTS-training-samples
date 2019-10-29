using System;
using Unity.Entities;

[Serializable]
public struct CarPosition : IComponentData
{
    public float Distance;
    public float Lane;
}

[Serializable]
public unsafe struct CarPositionStaticProperties : IComponentData
{
    public fixed float MergeSpace[4];
}
