using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class Utility
{
    public static int2 WorldPositionToGridCoordinates(float3 worldPos, float2 cellSize)
    {
        float2 worldPos2D = new float2(worldPos.x, worldPos.z);
        return new int2(Mathf.FloorToInt(worldPos2D.x / cellSize.x), Mathf.FloorToInt(worldPos2D.y / cellSize.y));
    }
}
