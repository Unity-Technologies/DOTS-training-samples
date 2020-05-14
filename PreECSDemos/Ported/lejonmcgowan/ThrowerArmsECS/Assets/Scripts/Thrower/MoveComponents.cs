using Unity.Entities;
using Unity.Mathematics;

public struct Velocity: IComponentData
{
    public float3 Value;

    public static implicit operator Velocity(float3 v) => new Velocity
    {
        Value = v
    };

    public static implicit operator float3(Velocity c) => c.Value;


}

public struct Acceleration: IComponentData
{
    public float3 Value;

    public static implicit operator Acceleration(float3 v) => new Acceleration
    {
        Value = v
    };

    public static implicit operator float3(Acceleration c) => c.Value;
}

public struct DestroyBoundsX : IComponentData
{
    public float2 Value;
    
    public static implicit operator DestroyBoundsX(float2 b) => new DestroyBoundsX()
    {
        Value = b
    };

    public static implicit operator float2(DestroyBoundsX b) => b.Value;

}


public struct DestroyBoundsY : IComponentData
{
    public float Value;
    
    public static implicit operator DestroyBoundsY(float b) => new DestroyBoundsY()
    {
        Value = b
    };

    public static implicit operator float(DestroyBoundsY b) => b.Value;

}

public struct SpawnerBoundsX : IComponentData
{
    public float2 Value;
}
