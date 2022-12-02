using Unity.Entities;
using Unity.Mathematics;

public struct TileComponent : IComponentData
{
    public float2 position;

    public int2 gridIndex;

    //TODO: go with north / west / south / east tile, should be able to be null
    //Also a reference to walls around, only needs a bit mask
    public int2 nextTile;

    public int2 southTile;
    public int2 northTile;
    public int2 westTile;
    public int2 eastTile;
}
