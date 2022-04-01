using Unity.Entities;
using Unity.Mathematics;

[InternalBufferCapacity(16)]
public struct StationPositionBufferElement : IBufferElementData
{
    public float positionAlongRail;
}
