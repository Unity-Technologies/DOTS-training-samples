using Unity.Entities;
using Unity.Mathematics;

public struct Particle : IComponentData
{
    public float life;
    public float lifeTime;
    public float3 velocity;
    public bool stuck;
    public float size;
}