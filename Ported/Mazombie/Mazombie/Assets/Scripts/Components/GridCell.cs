using System;
using Unity.Entities;

[Flags]
public enum WallFlags : byte
{
    None = 0,
    North = 1 << 0,
    South = 1 << 1,
    East  = 1 << 2,
    West  = 1 << 3,
    
    All = North | South | East | West,
}

public struct GridCell : IBufferElementData
{
    public byte wallFlags;
}