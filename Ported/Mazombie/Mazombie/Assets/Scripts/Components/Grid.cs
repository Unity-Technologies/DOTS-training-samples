
using System;
using Unity.Entities;

[Flags]
public enum WallFlags : byte
{
    North = 1 << 0,
    South = 1 << 1,
    East  = 1 << 2,
    West  = 1 << 3
}

public struct Grid : IBufferElementData
{
    public byte wallFlags;
}