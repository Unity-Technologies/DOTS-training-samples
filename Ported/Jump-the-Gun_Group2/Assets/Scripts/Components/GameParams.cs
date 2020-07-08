using Unity.Entities;
using Unity.Mathematics;

[GenerateAuthoringComponent]
public struct GameParams : IComponentData
{
    public Entity TilePrefab;
    public Entity CannonBallPrefab;
    public float TerrainMin;
    public float TerrainMax;
    public int2 TerrainDimensions;
    public Entity CannonPrefab;
    public int CannonCount;
    public float CannonCooldown;
    public float4 colorA;
    public float4 colorB;
    public Entity PlayerPrefab;
}
