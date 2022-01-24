

using System;
using Unity.Entities;

[Flags]
public enum Dir
{
    East = 1,
    West = 2,
    North = 4,
    South = 8
}

public struct Direction : IComponentData
{
    public Dir Value;
}
