using Unity.Entities;
using Unity.Mathematics;

struct PlatformSpawnerComponent : IComponentData
{
    public Entity PlatformPrefab;
    public float3 Position;
    public float Rotation;
}
