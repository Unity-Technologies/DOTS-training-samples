using System;
using Unity.Entities;

// TODO: Maybe split into dynamic (cat/arrow) and static cell data (wall/hole/homebase)
public struct CellComponent : IBufferElementData
{
    public CellData data;
}

// TODO: Confuse type cell?
[Flags]
public enum CellData : byte
{
    WallWest = 1,
    WallEast = 2,
    WallNorth = 4,
    WallSouth = 8,
    Hole = 16,
    HomeBase = 32,
    Eater = 64,
    Arrow = 128
}