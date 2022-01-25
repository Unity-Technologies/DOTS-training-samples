
using System;
using Unity.Entities;

[Flags]
public enum DirectionEnum : byte
{
    None = 0,
    North = 1,
    East = 2,
    South = 4,
    West = 8,
    Hole = 16
}

public struct Direction: IComponentData
{
    public DirectionEnum Value;
}
