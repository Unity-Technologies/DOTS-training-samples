using Unity.Entities;
using Unity.Rendering;
using UnityEngine;

[GenerateAuthoringComponent]
public struct BoardSpawner : IComponentData
{
    public int boardSize;
    public int matchDuration;
    public int pauseBetweenMatches;
    public Entity tilePrefab;
    public Entity wallPrefab;
    public Entity goalPrefab;
    public Vector2Int maxWallsRange;
    public int maxHoles;
    public uint randomSeed;

    public Entity catSpawnerPrefab;
    public Entity ratSpawnerPrefab;
}
