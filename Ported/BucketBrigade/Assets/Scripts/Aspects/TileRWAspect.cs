using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

readonly partial struct TileRWAspect : IAspect<TileRWAspect>
{
    public readonly Entity Self;

    private readonly RefRW<Tile> Tile;

    public int2 Position
    {
        get => Tile.ValueRW.Position;
        set => Tile.ValueRW.Position = value;
    }
    
    public float Heat
    {
        get => Tile.ValueRW.Heat;
        set => Tile.ValueRW.Heat = value;
    }
}
