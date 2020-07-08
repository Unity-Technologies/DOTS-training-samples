using Unity.Entities;
using Unity.Mathematics;

[GenerateAuthoringComponent]
public struct GameParams : IComponentData
{
    public Entity TilePrefab;
    public Entity CannonBallPrefab;
    public float2 TerrainHeightRange;
    public float2 TerrainDimensions;
}
