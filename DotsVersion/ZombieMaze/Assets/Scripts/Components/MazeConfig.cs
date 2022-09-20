using Unity.Entities;
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

    public Vector2Int GetRandomTilePosition()
    {
        return new Vector2Int(Random.Range(0, Width), Random.Range(0, Height));
    }

    public Vector2Int GetRandomTilePositionInside(int width, int height)
    {
        return new Vector2Int(Random.Range(width, Width - width), Random.Range(height, Height - height));
    }

    public int Get1DIndex(int xIndex, int yIndex)
    {
        return xIndex + yIndex * Width;
    }
}
