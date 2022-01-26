
using System;
using Unity.Entities;
using Unity.Mathematics;

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

static class DirectionEnumExtensions
{
    public static int2 ToVector2(this DirectionEnum dir)
    {
        // Use bitfield to determine direction we are going
        int2 dirVec = new int2(new bool2((dir & DirectionEnum.West) != 0, (dir & DirectionEnum.South) != 0));
        dirVec -= new int2(new bool2((dir & DirectionEnum.East) != 0, (dir & DirectionEnum.North) != 0));
        return dirVec;
    }

    public static int3 ToVector3(this DirectionEnum dir)
    {
        return new int3((dir & DirectionEnum.West) != 0 ? 1 : (dir & DirectionEnum.East) != 0 ? -1 : 0,
            (dir & DirectionEnum.South) != 0 ? 1 : (dir & DirectionEnum.North) != 0 ? -1 : 0,
            (dir & DirectionEnum.Hole) != 0 ? -1 : 0);
    }

    public static DirectionEnum Reverse(this DirectionEnum dir)
    {
        return dir switch
        {
            DirectionEnum.North => DirectionEnum.South,
            DirectionEnum.East => DirectionEnum.West,
            DirectionEnum.South => DirectionEnum.North,
            DirectionEnum.West => DirectionEnum.East,
            _ => DirectionEnum.None
        };
    }

}
