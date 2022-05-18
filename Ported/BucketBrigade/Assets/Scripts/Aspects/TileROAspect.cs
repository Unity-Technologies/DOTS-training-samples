using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;

// TODO: rename
readonly partial struct TileROAspect : IAspect<TileROAspect>
{
    public readonly Entity Self;
    public readonly RefRW<URPMaterialPropertyBaseColor> BaseColor;
    public readonly RefRW<NonUniformScale> Scale;

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
