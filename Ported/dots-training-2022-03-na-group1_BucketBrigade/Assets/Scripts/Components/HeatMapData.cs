using Unity.Entities;
using Unity.Mathematics;

public struct HeatMapData : IComponentData
{
    public int width;
    public float heatSpeed;
    public float4 startColor;
    public float4 finalColor;
}
