using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class Utility
{
    public static int2 WorldPositionToGridCoordinates(float2 worldPos, float2 cellSize)
    {
        float2 worldPos2D = new float2(worldPos.x, worldPos.y);
        return new int2(Mathf.FloorToInt(worldPos2D.x / cellSize.x), Mathf.FloorToInt(worldPos2D.y / cellSize.y));
    }

    public static float2 GridCoordinatesToWorldPos(int2 gridCoord, float2 cellSize)
    {
        return new float2(gridCoord.x * cellSize.x + cellSize.x / 2, gridCoord.y * cellSize.y + cellSize.y / 2);
    }
}
