using Unity.Entities;
using Unity.Mathematics;

/// <summary>
/// Tag to identify arrow spawners.
/// </summary>
public struct LbBoardGenerator : IComponentData
{
    public byte SizeX;
    public byte SizeY;
    public float YNoise;

    public Entity CellPrefab;
    public Entity WallPrefab;

    public Entity Player1Homebase;
    public Entity Player2Homebase;
    public Entity Player3Homebase;
    public Entity Player4Homebase;

    public Entity SpawnerPrefab;
    public int AdditionalSpawners;

    public Entity Player1Cursor;
    public Entity Player2Cursor;
    public Entity Player3Cursor;
    public Entity Player4Cursor;
}