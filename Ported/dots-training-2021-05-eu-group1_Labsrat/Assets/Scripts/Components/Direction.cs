using System;
using Unity.Entities;
using Unity.Mathematics;

public struct ForcedDirection : IComponentData
{
    public Cardinals Value;
}

public struct Direction : IComponentData
{
    public Cardinals Value;

    public Direction(Cardinals c)
    {
        Value = c;
    }
    
    public static Cardinals RotateRight(Cardinals c)
    {
        switch (c)
        {
            default:
            case Cardinals.None:
                return Cardinals.None;
            case Cardinals.North:
                return Cardinals.East;
            case Cardinals.West:
                return Cardinals.North;
            case Cardinals.South:
                return Cardinals.West;
            case Cardinals.East:
                return Cardinals.South;
        }
    }

    public float2 getDirection()
    {
        return getDirection(Value);
    }


    public static float2 getDirection(Cardinals c)
    {
        switch (c)
        {
            default:
            case Cardinals.None:
                return new float2(0, 0);
            case Cardinals.North:
                return new float2(0, 1);
            case Cardinals.West:
                return new float2(1, 0);
            case Cardinals.South:
                return new float2(0, -1);
            case Cardinals.East:
                return new float2(-1, 0);
        }
    }

    public static float GetAngle(Cardinals direction)
    {
        switch (direction)
        {
            default:
            case Cardinals.None:
            case Cardinals.North:
                return 0;
            case Cardinals.West:
                return math.radians(90);
            case Cardinals.South:
                return math.radians(180);
            case Cardinals.East:
                return math.radians(270);

        }
    }

    public static Cardinals FromRandomDirection (int randDir)
    {
        switch(randDir)
        {
            default:
            case 0: return Cardinals.North;
            case 1: return Cardinals.West;
            case 2: return Cardinals.South;
            case 3: return Cardinals.East;
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
