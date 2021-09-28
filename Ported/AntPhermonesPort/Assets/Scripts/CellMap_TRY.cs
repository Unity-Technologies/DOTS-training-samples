using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;

public static class CellMap_TRY
{
    readonly static float2 worldLowerLeft = new float2(-5, -5);
    readonly static float2 worldUpperRight = new float2(5, 5);

    const int gridSize = 500;

    public static NativeArray<CellState> CellMap = new NativeArray<CellState>(gridSize * gridSize, Allocator.Persistent);

    public static void InitBorders()
    {
        for(int i = 0; i < gridSize; ++i)
        {
            CellMap[ConvertGridIndex2Dto1D(new int[] { i, 0 })] = CellState.IsObstacle;
            CellMap[ConvertGridIndex2Dto1D(new int[] { i, gridSize })] = CellState.IsObstacle;
            CellMap[ConvertGridIndex2Dto1D(new int[] { 0, i})] = CellState.IsObstacle;
            CellMap[ConvertGridIndex2Dto1D(new int[] { gridSize, i })] = CellState.IsObstacle;
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

    public static int ConvertGridIndex2Dto1D(int[] index)
    => index[1] * gridSize + index[0];
}
