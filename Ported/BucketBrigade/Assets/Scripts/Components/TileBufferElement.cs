using Unity.Entities;

[InternalBufferCapacity(1000)]
public struct TileBufferElement : IBufferElementData
{
    public Entity Tile;
}
