using Unity.Entities;
using Unity.Mathematics;

[GenerateAuthoringComponent]
public struct TileCoord : IComponentData
{
    public int2 Value;
}