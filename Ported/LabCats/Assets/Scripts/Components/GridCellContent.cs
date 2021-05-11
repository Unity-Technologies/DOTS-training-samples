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
    WallLeft,
    WallRight,
    WallDown,
    WallUp
}
