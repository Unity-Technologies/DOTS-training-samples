using Unity.Entities;
using Unity.Mathematics;

public struct HeatMapData : IComponentData
{
    public int mapSideLength;
    public float maxTileHeight;
    public float heatPropagationSpeed;
    public int heatPropagationRadius;
    
    public float4 colorNeutral;
    public float4 colorCool;
    public float4 colorHot;
}
