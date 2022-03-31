using Unity.Entities;
using Unity.Mathematics;

[InternalBufferCapacity(32)]
public struct WaterPoolInfo : IBufferElementData
{
    public Entity WaterPool;
    public float2 Position;
    public float Radius;
}