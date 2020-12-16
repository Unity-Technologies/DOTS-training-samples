using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;

public struct TileSpawner : IComponentData
{
    public Entity TilePrefab;
    public Entity RockPrefab;
    public Entity SiloPrefab;
    public Entity Settings;
    
    public int2 GridSize;
    public float2 TileSize;
    
    public int Attempts;
    public int StoreCount;
}