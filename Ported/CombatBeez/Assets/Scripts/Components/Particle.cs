using Unity.Entities;

public struct Particle : IComponentData
{
    public float TimeSinceLastUpdate;

    public ParticleType Type;
    public ParticleState State;
}
