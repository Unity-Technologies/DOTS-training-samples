using Unity.Entities;
using Unity.Mathematics;

struct TileGridConfig : IComponentData
{
    public int Size;
    public float CellSize;
    public Entity TilePrefab;
    public float4 GrassColor;
    public float4 LightFireColor;
    public float4 MediumFireColor;
    public float4 IntenseFireColor;
    
    public int Spacing;
    public int OuterSize;
    public int NbOfWaterTiles;
    
    public float4 LightWaterColor;
    public float4 MediumWaterColor;
    public float4 IntenseWaterColor;
}
