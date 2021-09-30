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

    public void InitLOSData(int2 food)
    {
        // init Nest
        int2 nest = new int2 { x = grid.gridDimLength / 2, y = grid.gridDimLength / 2 };
        int end = grid.gridDimLength - 1;

        for (int i = 0; i < grid.gridDimLength; ++i)
        {
            ComputeLOS(new int2 { x = i,   y = 0   }, nest, false);
            ComputeLOS(new int2 { x = i,   y = end }, nest, false);
            ComputeLOS(new int2 { x = 0,   y = i   }, nest, false);
            ComputeLOS(new int2 { x = end, y = i   }, nest, false);
            ComputeLOS(new int2 { x = i,   y = 0   }, food, true);
            ComputeLOS(new int2 { x = i,   y = end }, food, true);
            ComputeLOS(new int2 { x = 0,   y = i   }, food, true);
            ComputeLOS(new int2 { x = end, y = i   }, food, true);
        }

        // init Food
    }

    private void ComputeLOS(int2 source, int2 target, bool isFood)
    {
        float diffX = Mathf.Abs(source.x - target.x);
        float diffY = Mathf.Abs(source.y - target.y);

        float incX, incY;

        if (diffX >= diffY)
        {
            incX = source.x < target.x ? -1 : 1;
            incY = diffY / diffX * (source.y < target.y ? -1f : 1f);
        }
        else
        {
            incX = diffX / diffY * (source.x < target.x ? -1f : 1f);
            incY = source.y < target.y ? -1 : 1;
        }

        float x = target.x;
        float y = target.y;

        for (; true; x += incX, y += incY)
        {
            var xy = new int2 { x = (int)x, y = (int)y };
            if (!StampCellLOS(xy, isFood) || xy.x == source.x && xy.y == source.y)
                return;
        }
    }

    private bool StampCellLOS(int2 xy, bool isFood)
    {
        var state = Get(xy);

        if (state != CellState.IsObstacle)
        {
            // already stamped?
            if (state != CellState.HasLineOfSightToBoth && state != (isFood ? CellState.HasLineOfSightToFood : CellState.HasLineOfSightToNest))
            {
                if (isFood)
                    state = state == CellState.HasLineOfSightToNest ? CellState.HasLineOfSightToBoth : CellState.HasLineOfSightToFood;
                else
                    state = state == CellState.HasLineOfSightToFood ? CellState.HasLineOfSightToBoth : CellState.HasLineOfSightToNest;

                Set(xy, state);
            }

            return true;
        }

        return false;
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

        if (cellIndex < 0 || cellIndex >= cellmap.Length)
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

    public void Set(int2 xy, CellState state)
    {
        if (xy.x >= 0 && xy.x < grid.gridDimLength && xy.y >= 0 && xy.y < grid.gridDimLength)
            cellmap.ElementAt(xy.y * grid.gridDimLength + xy.x).state = state;
    }

    public void Set(int index, CellState state)
    {
        if (index >= 0 && index < cellmap.Length)
            cellmap.ElementAt(index).state = state;
    }

    public CellState Get(int x, int y)
    {
        int pos = y * grid.gridDimLength + x;
        if (pos < 0 || pos >= cellmap.Length)
            return CellState.IsObstacle;
        return cellmap[pos].state;
    }

    public CellState Get(int2 xy)
    {
        int pos = xy.y * grid.gridDimLength + xy.x;
        if (pos < 0 || pos >= cellmap.Length)
            return CellState.IsObstacle;
        return cellmap[pos].state;
    }
}
