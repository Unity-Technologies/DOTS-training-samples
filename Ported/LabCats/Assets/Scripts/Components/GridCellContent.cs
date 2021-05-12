using Unity.Entities;
using System;


public struct GridCellContent : IBufferElementData
{
    public GridCellType Type;
    public WallBoundaries Walls;

    public static int Get1DIndexFromGridPosition(GridPosition position, int numberColumns)
    {
        return position.X + position.Y * numberColumns;
    }

    public static int Get1DIndexFromGridPosition(int x, int y, int numberColumns)
    {
        return x + y * numberColumns;
    }

    public static int GetRowIndexFrom1DIndex(int index, int numberColumns)
    {
        return index / numberColumns;
    }

    public static int GetColumnIndexFrom1DIndex(int index, int numberColumns)
    {
        return index % numberColumns;
    }
}

public enum GridCellType : byte
{
    None,
    Goal,
    Hole,
    ArrowLeft,
    ArrowRight,
    ArrowDown,
    ArrowUp
}

[Flags]
public enum WallBoundaries : byte
{
    WallLeft = 1,
    WallRight = 2,
    WallDown = 4,
    WallUp = 8,
    WallAll = WallLeft|WallRight|WallDown|WallUp
}
