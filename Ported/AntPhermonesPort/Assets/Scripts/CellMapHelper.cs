using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public struct CellMapHelper
{
    float2 worldLowerLeft;
    float2 worldUpperRight;

    DynamicBuffer<CellMap> cellmap;
    int gridSize;

    public CellMapHelper(DynamicBuffer<CellMap> _cellmap, int _gridSize, float _worldSize)
    {
        cellmap = _cellmap;
        gridSize = _gridSize;

        // World centered at 0,0
        var halfSize = _worldSize / 2;
        worldLowerLeft = new float2(-halfSize, -halfSize);
        worldUpperRight = new float2(halfSize, halfSize);
    }

    public bool IsInitialized()
    {
        return cellmap.Length > 0;
    }

    public void InitCellMap()
    {
        cellmap.Length = gridSize * gridSize;

        for (int i = 0; i < cellmap.Length - 1; ++i)
        {
            cellmap.ElementAt(i).state = CellState.Empty;
        }
    }

    public void InitBorders()
    {
        for(int i = 0; i < gridSize; ++i)
        {
            Set(i, 0, CellState.IsObstacle);
            Set(i, gridSize -1, CellState.IsObstacle);
            Set(0, i, CellState.IsObstacle);
            Set(gridSize -1, i, CellState.IsObstacle);
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

    public void StampPattern(int x, int y, NativeArray<int2> pattern)
    {
        foreach (var pt in pattern)
        {
            for (int px = pt.x; px <= pt.y; ++px)
            {
                Set(x + px, y, CellState.IsObstacle);
            }
        }
    }

    /// <summary>
    /// Returns the nearest index to a point in world space. The originOffset is used
    /// to align the [0,0] index with the start of the grid (since the grid origin in
    /// worldspace might be [-5.0, -5.0] for example if the plane of size 5 is at [0,0])
    /// </summary>
    /// <param name="xy"></param>
    /// <param name="originOffset"></param>
    /// <returns></returns>
    public int GetNearestIndex(float2 xy)
    {
        if(xy.x < worldLowerLeft.x || xy.y < worldLowerLeft.y || xy.x > worldUpperRight.x || xy.y > worldUpperRight.y)
        {
            //TBD: Warnings are disabled currently because ant move might jump out past the boundary cell
            //Debug.LogError("[Cell Map] Trying to get index out of range");
            return -1;
        }

        float2 gridRelativeXY = xy - worldLowerLeft;

        float2 gridIndexScaleFactor = (worldUpperRight - worldLowerLeft) / gridSize;
        float2 gridIndexXY = gridRelativeXY/gridIndexScaleFactor;

        return ConvertGridIndex2Dto1D(gridIndexXY);
    }

    public void WorldToCellSpace(ref float x, ref float y)
    {
        if (x < worldLowerLeft .x) x = worldLowerLeft .x;
        if (x > worldUpperRight.x) x = worldUpperRight.x;
        if (y < worldLowerLeft .y) y = worldLowerLeft .y;
        if (y > worldUpperRight.y) y = worldUpperRight.y;

        float2 gridRelativeXY = new float2 { x = x, y = y } - worldLowerLeft;

        float2 gridIndexScaleFactor = (worldUpperRight - worldLowerLeft) / gridSize;

        x = gridRelativeXY.x / gridIndexScaleFactor.x;
        y = gridRelativeXY.y / gridIndexScaleFactor.y;

    }

    public CellState GetCellStateFrom2DPos(float2 xy)
    {
        int cellIndex = GetNearestIndex(xy);
        if (cellIndex < 0 || cellIndex > cellmap.Length)
        {
            //Debug.LogError(string.Format("[Cell Map] Position is outside cell map {0}, {1}", xy.x, xy.y));
            return CellState.IsObstacle;
        }

        return cellmap[cellIndex].state;
    }

    public int ConvertGridIndex2Dto1D(float2 index)
        => Mathf.RoundToInt(index.y) * gridSize + Mathf.RoundToInt(index.x);

    public void Set(int x, int y, CellState state)
    {
        if (x >= 0 && x < gridSize && y >= 0 && y < gridSize)
            cellmap.ElementAt(y * gridSize + x).state = state;
    }

    public CellState Get(int x, int y)
    {
        return cellmap[y * gridSize + x].state;
    }
}
