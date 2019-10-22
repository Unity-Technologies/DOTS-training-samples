using Unity.Entities;

public struct GravityStrength : IComponentData
{
    public float Value;
}

public struct FlyingTag : IComponentData
{}
