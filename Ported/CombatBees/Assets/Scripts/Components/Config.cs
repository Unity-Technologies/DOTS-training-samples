using Unity.Entities;
using Unity.Mathematics;

public struct Config : IComponentData
{
    public Entity beePrefab;
    public Entity particlePrefab;
    public int startBeeCount;
    public int beesPerResource;
    public float minimumBeeSize;
    public float maximumBeeSize;
    public float3 gravity;
    public float3 fieldSize;
    public Entity resourceSpawn;
    public Entity resourcePrefab;
    public int resourceCount;
}