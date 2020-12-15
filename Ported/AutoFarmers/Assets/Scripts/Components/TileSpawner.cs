using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;

public struct TileSpawner : IComponentData
{
    public Entity TilePrefab;
    public int2 GridSize;
    public float2 TileSize;
}