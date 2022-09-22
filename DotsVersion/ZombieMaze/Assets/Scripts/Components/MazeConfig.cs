using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public struct MazeConfig : IComponentData
{
    public int Width;
    public int Height;
    public int PillsToSpawn;
    public int ZombiesToSpawn;
    public int OpenStrips;
    public int MazeStrips;
    public int MovingWallsToSpawn;
    public int MovingWallSize;
    public float MovingWallMinMoveSpeedInSeconds;
    public float MovingWallMaxMoveSpeedInSeconds;
    public int MovingWallMinTilesToMove;
    public int MovingWallMaxTilesToMove;

    public bool DestroyAllToRemake;
    public bool RebuildMaze;
    public bool SpawnPills;

    public int2 GetRandomTilePosition()
    {
        return new int2(UnityEngine.Random.Range(0, Width), UnityEngine.Random.Range(0, Height));
    }

    public int2 GetRandomTilePositionInside(int width, int height)
    {
        return new int2(UnityEngine.Random.Range(width, Width - width), UnityEngine.Random.Range(height, Height - height));
    }

    public int Get1DIndex(int xIndex, int yIndex)
    {
        return xIndex + yIndex * Width;
    }
}
