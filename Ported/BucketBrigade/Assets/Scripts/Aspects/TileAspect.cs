using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;

readonly partial struct TileAspect : IAspect<TileAspect>
{
    public readonly Entity Self;
    public readonly RefRW<URPMaterialPropertyBaseColor> BaseColor;
    public readonly RefRW<NonUniformScale> Scale;

    private readonly RefRO<Tile> Tile;

    public int2 Position
    {
        get => Tile.ValueRO.Position;
    }
}
