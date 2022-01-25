

using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

[GenerateAuthoringComponent]
public struct Config : IComponentData
{
    public float MouseMovementSpeed;
    public float CatMovementSpeed;

    public float MouseSpawnRate;
    public float2 MouseSpawnCooldown;
    public int MiceToSpawnPerSpawner;
    public int MiceSpawnerInMap;
    
    public uint MapSeed;
    public int MapWidth;
    public int MapHeight;

    public int CatsInMap;
    
    public Entity CatPrefab;
    public Entity MousePrefab;
    public Entity ExitPrefab;
    public Entity ArrowPrefab;
}
