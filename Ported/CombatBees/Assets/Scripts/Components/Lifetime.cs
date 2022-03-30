using Unity.Entities;
using Unity.Mathematics;

public struct Lifetime : IComponentData
{
    public float Value;
    public float Duration;
}
