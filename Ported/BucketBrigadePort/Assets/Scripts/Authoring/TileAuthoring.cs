using Unity.Entities;
using Unity.Mathematics;

[GenerateAuthoringComponent]
public struct TileAuthoring : IComponentData
{
    public Prefab Tile;
    public int2 Id;
}
