using Unity.Entities;
using Unity.Rendering;
using UnityEngine;

[GenerateAuthoringComponent]
public struct BoardSpawner : IComponentData
{
    public int boardSize;
    public Entity tilePrefab;
    public Entity wallPrefab;
    public Entity goalPrefab;
    public Vector2Int maxWallsRange;
    public int maxHoles;

    public Entity catSpawnerPrefab;
    public Entity ratSpawnerPrefab;
}
