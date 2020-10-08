using System;
using Unity.Entities;

[InternalBufferCapacity(3)]
public struct PlayerArrow : IBufferElementData
{
    public int TileIndex;
}
