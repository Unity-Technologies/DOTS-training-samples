

using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public enum CollisionMode
{
    Cell,
    Distance
}

[GenerateAuthoringComponent]
public struct Config : IComponentData
{
    public float MouseMovementSpeed;
    public float CatMovementSpeed;
    public float CreatureFallSpeed;

    public float MouseSpawnRate;
    public float2 MouseSpawnCooldown;
    public int MiceToSpawnPerSpawner;
    public int MiceSpawnerInMap;
    
    public uint MapSeed;
    public int MapWidth;
    public int MapHeight;

    public float MapWallFrequency;
    public float MapHoleFrequency;

    public int CatsInMap;

    public CollisionMode CollisionMode;
    
    public Entity CatPrefab;
    public Entity MousePrefab;
    public Entity ExitPrefab;
    public Entity ArrowPrefab;

    public int MaxArrowsPerPlayer;
}
