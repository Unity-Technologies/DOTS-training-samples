﻿
using Unity.Entities;

public struct GameConfig : IComponentData
{
    public Entity tile;
    public Entity wallPrefab;
    public Entity playerSpawnPrefab;
    public Entity playerPrefab;
    public int mazeSize;
}