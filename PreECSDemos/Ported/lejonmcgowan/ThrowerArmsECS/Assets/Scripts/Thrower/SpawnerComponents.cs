using Unity.Entities;
using Unity.Mathematics;

public struct RockSpawnComponent: IComponentData
{
    public float spawnTime;
    public float spawnTimeRemaining;
    public float2 radiusRanges;
    public float3 spawnVelocity;
    public Random rng;
    public Entity prefab;
}

public struct CanSpawnComponent: IComponentData
{
    public float spawnTime;
    public float spawnTimeRemaining;
    public float2 yRanges;
    public float2 bounds;
    public float zSpawnPos;
    public float xSpawnPos;
    public float3 spawnVelocity;
    public Random rng;
    public Entity prefab;
}