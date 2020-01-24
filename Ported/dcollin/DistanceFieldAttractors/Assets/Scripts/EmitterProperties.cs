using Unity.Entities;
using Unity.Mathematics;

public struct EmitterPropetiers : IComponentData
{
    public float Attraction;
    public float SpeedStretch;
    public float Jitter;
    public float4 SurfaceColor;
    public float4 InteriorColor;
    public float4 ExteriorColor;
    public float ExteriorColorDist;
    public float InteriorColorDist;
    public float ColorStiffness;
    public int SpawnCount;
}