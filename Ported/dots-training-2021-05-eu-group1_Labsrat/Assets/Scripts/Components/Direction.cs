using System;
using Unity.Entities;
using Unity.Mathematics;

public struct Direction : IComponentData
{
    public Cardinals Value;

    public Direction(Cardinals c)
    {
        Value = c;
    }

    public float2 getDirection()
    {
        switch (Value)
        {
            default:
            case Cardinals.None:
                return new float2(0, 0);
            case Cardinals.North:
                return new float2(0, 1);
            case Cardinals.West:
                return new float2(-1, 0);
            case Cardinals.South:
                return new float2(0, -1);
            case Cardinals.East:
                return new float2(1, 0);
        }
    }
}


[Flags]
public enum Cardinals
{
    None = 0,
    North = 1,
    West = 2,
    South = 4,
    East = 8
}
