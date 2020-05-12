using Unity.Entities;
using Unity.Mathematics;

public struct CanVelocity: IComponentData
{
    public float3 Value;

    public static implicit operator CanVelocity(float3 velocity) => new CanVelocity()
    {
        Value = velocity
    };

    public static implicit operator float3(CanVelocity c) => c.Value;

}

public struct CanDestroyBounds : IComponentData
{
    public float2 Value;
    
    public static implicit operator CanDestroyBounds(float2 b) => new CanDestroyBounds()
    {
        Value = b
    };

    public static implicit operator float2(CanDestroyBounds b) => b.Value;

}

public struct CanReservedTag : IComponentData
{
}

public struct CanReserveRequest : IComponentData
{
    public Entity canRef;
    public Entity armRef;

    //used for tie breaking purposes
    public float3 armPos;
}