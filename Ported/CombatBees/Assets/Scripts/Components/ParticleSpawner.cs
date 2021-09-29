using Unity.Entities;
using Unity.Mathematics;

public struct ParticleSpawner : IComponentData
{
    public Entity Prefab;
    public float3 Position;
    public float3 Direction;
    public float Spread;
    public float Lifetime;
    public int Count;
}
