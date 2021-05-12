using Unity.Mathematics;
using UnityEngine;

public static class CoordUtils
{
    public static readonly float3 BoardOffset = new float3(-0.5f, 0.0f, -0.5f);

    public static int2 WorldToBoardPosition(float3 worldPos)
    {
        float3 localPos = worldPos - BoardOffset;
        return new int2((int)math.floor(localPos.x), (int)math.floor(localPos.z));
    }

    public static int2 WorldOffsetToBoardPosition(float2 worldOffset)
    {
        return new int2((int)math.floor(worldOffset.x - BoardOffset.x), (int)math.floor(worldOffset.y - BoardOffset.z));
    }

    public static int2 ClampPos(int2 coord, int2 bounds)
    {
        return math.clamp(coord, new int2(0,0), bounds - 1);
    }

    public static float2 BoardPosToWorldOffset(int2 boardPos)
    {
        float2 worldOffset = new float2((float)boardPos.x, (float)boardPos.y);
        return worldOffset;
    }

    public static float3 BoardPosToWorldPos(int2 boardPos, float height)
    {
        float2 worldOffset = BoardPosToWorldOffset(boardPos);
        return new float3(worldOffset.x, height, worldOffset.y);
    }

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
