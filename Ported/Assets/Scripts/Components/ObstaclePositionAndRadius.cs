using Unity.Entities;
using Unity.Mathematics;

[InternalBufferCapacity(0)]
public readonly struct ObstaclePositionAndRadius : IBufferElementData
{
    public readonly float radius;
    public readonly float2 position;

    public readonly bool IsValid => radius > 0f;

    public ObstaclePositionAndRadius(in float radius = 0, in float x = float.MinValue, in float y = float.MinValue)
    {
        this.radius = radius;
        position = new float2(x,y);
    }
}