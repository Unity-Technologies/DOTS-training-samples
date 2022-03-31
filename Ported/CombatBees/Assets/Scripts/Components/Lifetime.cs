using Unity.Entities;
using Unity.Mathematics;

public struct Lifetime : IComponentData
{
    public float Value;
    public float Duration;
}

// Replicating the awkward bee logic
public struct BeeLifetime : IComponentData
{
    public float Value;
    public byte NewlyDead;
}
