using Unity.Entities;
using Unity.Mathematics;

/// <summary>
/// Tag to identify arrow spawners.
/// </summary>
public struct LbArrowSpawner : IComponentData
{
    public Entity Prefab;
    public byte PlayerId;
    public byte Direction;
    public float3 Location;
}
