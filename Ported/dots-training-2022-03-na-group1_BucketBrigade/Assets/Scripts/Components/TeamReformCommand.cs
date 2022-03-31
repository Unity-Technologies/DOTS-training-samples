using Unity.Entities;
using Unity.Mathematics;

[InternalBufferCapacity(32)]
public struct TeamReformCommand : IBufferElementData
{
    public Entity Team;
}