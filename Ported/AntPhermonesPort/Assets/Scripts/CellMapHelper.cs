using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public static class CellMapHelper
{
    readonly static float2 worldLowerLeft = new float2(-5, -5);
    readonly static float2 worldUpperRight = new float2(5, 5);

    const int gridSize = 500;

    public static void InitCellMap(DynamicBuffer<CellMap> cellmap)
    {
        cellmap.Length = gridSize * gridSize;
        InitBorders(cellmap);
    }

    static void InitBorders(DynamicBuffer<CellMap> cellmap)
    {
        for(int i = 0; i < gridSize; ++i)
        {
            Set(cellmap, i, 0, CellState.IsObstacle);
            Set(cellmap, i, gridSize -1, CellState.IsObstacle);
            Set(cellmap, 0, i, CellState.IsObstacle);
            Set(cellmap, gridSize -1, i, CellState.IsObstacle);
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
    public static int GetNearestIndex(float2 xy)
    {
        if(xy.x < worldLowerLeft.x || xy.y < worldLowerLeft.y || xy.x > worldUpperRight.x || xy.y > worldUpperRight.y)
        {
            Debug.LogError("[Cell Map] Trying to get index out of range");
            return -1;
        }

        float2 gridRelativeXY = xy - worldLowerLeft;

        float2 gridIndexScaleFactor = (worldUpperRight - worldLowerLeft) / gridSize;
        float2 gridIndexXY = gridRelativeXY/gridIndexScaleFactor;

        return ConvertGridIndex2Dto1D(gridIndexXY);
    }

    public static int ConvertGridIndex2Dto1D(float2 index)
        => Mathf.RoundToInt(index.y * gridSize + index.x);

    public static void Set(DynamicBuffer<CellMap> cellmap, int x, int y, CellState state)
    {
        cellmap.ElementAt(y * gridSize + x).state = state;
    }

    public static CellState Get(DynamicBuffer<CellMap> cellmap, int x, int y)
    {
        return cellmap[y * gridSize + x].state;
    }

}
