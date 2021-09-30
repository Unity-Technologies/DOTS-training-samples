using Unity.Entities;
using Unity.Mathematics;

public struct ParticleSpawner : IComponentData
{
    public Entity Prefab;
    public float3 Position;
    public float Lifetime;
    public float Speed;
    public int Count;
}
