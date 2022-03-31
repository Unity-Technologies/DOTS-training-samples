using Unity.Entities;
using Unity.Mathematics;

[InternalBufferCapacity(32)]
public struct DumpBucketCommand : IBufferElementData
{
    public Entity Worker;
    public int fireTileIndex;
}