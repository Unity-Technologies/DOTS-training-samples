using Unity.Entities;

public struct GravityStrength : ISharedComponentData
{
    public float Value;
}

public struct FlyingTag : IComponentData
{}
