using Unity.Entities;
using Unity.Mathematics;

public struct BeeSpawnerComponent : IComponentData
{
    public HiveTag hiveTag;
    public int initialSpawnAmount;
    public Entity beePrefab;
    public float3 minBounds;
    public float3 maxBounds;
}