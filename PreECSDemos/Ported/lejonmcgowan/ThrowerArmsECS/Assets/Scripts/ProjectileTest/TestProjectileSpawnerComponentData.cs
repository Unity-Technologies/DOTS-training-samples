using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class TestProjectileSpawnerComponentData: IComponentData
{
    public float2 velocityRange;
    public float3 velocityDirection;
    
    public float2 lifetimeRange;
    public float2 spawnXRange;
    public float2 spawnYRange;
    
    public float  timeUntilSpawn;
    public float spawnTime;

    public Entity projectilePrefab;
}