using Unity.Entities;
using UnityEngine;

public struct MazeConfig : IComponentData
{
    public int Width;
    public int Height;
    public int PillsToSpawn;
    public int ZombiesToSpawn;

    public Vector2Int GetRandomTilePosition()
    {
        return new Vector2Int(Random.Range(0, Width), Random.Range(0, Height));
    }
}
