using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public struct CellMapHelper
{
    public GridHelper grid;
    public DynamicBuffer<CellMap> cellmap;

    public CellMapHelper(DynamicBuffer<CellMap> _cellmap, int _gridSize, float _worldSize)
    {
        // STAY
        cellmap = _cellmap;

        // NEW
        grid = new GridHelper(_gridSize, _worldSize);
    }

    public bool IsInitialized()
    {
        return cellmap.Length > 0;
    }

    public void InitCellMap()
    {
        cellmap.Length = grid.gridDimLength * grid.gridDimLength;

        for (int i = 0; i < cellmap.Length - 1; ++i)
        {
            cellmap.ElementAt(i).state = CellState.Empty;
        }
    }

    public void InitBorders()
    {
        for(int i = 0; i < grid.gridDimLength; ++i)
        {
            Set(i, 0, CellState.IsObstacle);
            Set(i, grid.gridDimLength - 1, CellState.IsObstacle);
            Set(0, i, CellState.IsObstacle);
            Set(grid.gridDimLength - 1, i, CellState.IsObstacle);
        }
    }

    private void ApplyCircleBoundaries(NativeArray<int2> pattern, int xc, int yc, int x, int y)
    {
        pattern[yc - y] = new int2(xc - x, xc + x);
        pattern[yc + y] = new int2(xc - x, xc + x);
        pattern[yc - x] = new int2(xc - y, xc + y);
        pattern[yc + x] = new int2(xc - y, xc + y);
    }

    // Function for circle-generation
    // using Bresenham's algorithm
    public NativeArray<int2> CreateCirclePattern(int r)
    {
        int xc = r;
        int yc = r;

        NativeArray<int2> pattern = new NativeArray<int2>(r * 2 + 1, Allocator.Temp);


        int x = 0, y = r;
        int d = 3 - 2 * r;

        ApplyCircleBoundaries(pattern, xc, yc, x, y);

        while (y >= x)
        {
            // for each pixel we will
            // draw all eight pixels
            x++;

            // check for decision parameter
            // and correspondingly
            // update d, x, y
            if (d > 0)
            {
                y--;
                d = d + 4 * (x - y) + 10;
            }
            else
            {
                d = d + 4 * x + 6;
            }

            ApplyCircleBoundaries(pattern, xc, yc, x, y);
        }

        return pattern;
    }

    public void StampPattern(int x, int y, NativeArray<int2> pattern, CellState cellState)
    {
        for (int dy = 0; dy < pattern.Length; dy++)
        {
            for (int px = pattern[dy].x; px <= pattern[dy].y; ++px)
            {
                Set(x + px, y + dy - pattern.Length / 2, cellState);
            }
        }
    }

    public CellState GetCellStateFrom2DPos(float2 xy, float debugLineTime = 0)
    {
        int cellIndex = grid.GetNearestIndex(xy);
        if(debugLineTime != 0) grid.DrawDebugRay(cellIndex, Color.blue, debugLineTime);

        if (cellIndex < 0 || cellIndex > cellmap.Length)
        {
            //Debug.LogError(string.Format("[Cell Map] Position is outside cell map {0}, {1}", xy.x, xy.y));
            return CellState.IsObstacle;
        }

        return cellmap[cellIndex].state;
    }

    public void Set(int x, int y, CellState state)
    {
        if (x >= 0 && x < grid.gridDimLength && y >= 0 && y < grid.gridDimLength)
            cellmap.ElementAt(y * grid.gridDimLength + x).state = state;
    }

    public void Set(int index, CellState state)
    {
        if (index >= 0 && index < cellmap.Length)
            cellmap.ElementAt(index).state = state;
    }

    public CellState Get(int x, int y)
    {
        return cellmap[y * grid.gridDimLength + x].state;
    }
}
