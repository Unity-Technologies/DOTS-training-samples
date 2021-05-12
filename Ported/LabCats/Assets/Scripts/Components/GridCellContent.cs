using Unity.Entities;
using System;


public struct GridCellContent : IBufferElementData
{
    public GridCellType Type;
    public WallBoundaries Walls;

    public static WallBoundaries GetWallBoundariesFromDirection(Dir direction)
    {
        switch (direction)
        {
            case Dir.Down:
                return WallBoundaries.WallDown;
            case Dir.Left:
                return WallBoundaries.WallLeft;
            case Dir.Right:
                return WallBoundaries.WallRight;
            case Dir.Up:
                return WallBoundaries.WallUp;
        }

        return WallBoundaries.NoWall;
    }

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

    public static int GetNeighbour1DIndexWithDirection(int index, Dir direction, int numberRows, int numberColumns)
    {
        int row = GetRowIndexFrom1DIndex(index, numberColumns);
        int col = GetColumnIndexFrom1DIndex(index, numberColumns);
        int neighbourIndex = -1;
        switch (direction)
        {
            case Dir.Up:
            {
                if (row > 0)
                    return Get1DIndexFromGridPosition(col, row - 1, numberColumns);
            }
                break;
            case Dir.Down:
            {
                if(row < numberRows-1)
                    return Get1DIndexFromGridPosition(col, row + 1, numberColumns);
            }
                break;
            case Dir.Left:
            {
                if(col > 0)
                    return Get1DIndexFromGridPosition(col-1, row, numberColumns);
            }
                break;
            case Dir.Right:
            {
                if(col < numberColumns-1)
                    return Get1DIndexFromGridPosition(col+1, row, numberColumns);
            }
                break;
        }

        return -1;
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
    NoWall = 0,
    WallLeft = 1,
    WallRight = 2,
    WallDown = 4,
    WallUp = 8,
    WallAll = WallLeft|WallRight|WallDown|WallUp
}
