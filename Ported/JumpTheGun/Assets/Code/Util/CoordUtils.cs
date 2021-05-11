using Unity.Mathematics;
using UnityEngine;

public static class CoordUtils
{
    public static int2 ToCoords(int index, int width, int height)
    {
        return new int2(index % width, index / width);
    }
    
    public static int ToIndex(int2 coords, int width, int height)
    {
        return coords.x + coords.y * width;
    }

    public static int2 WorldToBoardPosition(in float3 position, in BoardSize boardSize, in float3 boardWorldOffset)
    {
        float3 localPos = position - boardWorldOffset;
        float boardX = Mathf.Floor(localPos.x / (float)boardSize.Value.x);
        float boardY = Mathf.Floor(localPos.y / (float)boardSize.Value.y);
        return new int2((int)boardX, (int)boardY);
    }
}
