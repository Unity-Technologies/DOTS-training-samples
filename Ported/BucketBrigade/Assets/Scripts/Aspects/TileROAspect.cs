using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

readonly partial struct TileROAspect : IAspect<TileROAspect>
{
    public readonly Entity Self;

    private readonly RefRO<Tile> Tile;

    public int2 Position
    {
        get => Tile.ValueRO.Position;
    }
    
    public float Heat
    {
        get => Tile.ValueRO.Heat;
    }
}
