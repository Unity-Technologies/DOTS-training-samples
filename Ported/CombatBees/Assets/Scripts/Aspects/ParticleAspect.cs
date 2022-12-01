using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public readonly partial struct ParticleAspect : IAspect
{
    public readonly Entity self;
    private readonly TransformAspect transform;
    private readonly RefRW<Particle> particle;

    public float3 WorldPosition
    {
        get => transform.WorldPosition;
        set => transform.WorldPosition = value;
    }
    public float LocalScale
    {
        get => transform.LocalScale;
        set => transform.LocalScale = value;
    }
    public float Size
    {
        get => particle.ValueRO.size;
        set => particle.ValueRW.size = value;
    }
    public float Life
    {
        get => particle.ValueRO.life;
        set => particle.ValueRW.life = value;
    }
    public float LifeTime
    {
        get => particle.ValueRO.lifeTime;
        set => particle.ValueRW.lifeTime = value;
    }
    public float3 Velocity
    {
        get => particle.ValueRO.velocity;
        set => particle.ValueRW.velocity = value;
    }
    public bool Stuck
    {
        get => particle.ValueRO.stuck;
        set => particle.ValueRW.stuck = value;
    }
}