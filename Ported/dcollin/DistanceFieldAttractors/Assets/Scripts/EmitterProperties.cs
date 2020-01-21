using Unity.Entities;
using Unity.Mathematics;

public struct EmitterPropetiers : ISharedComponentData
{
    public float Attraction;
    public float SpeedStretch;
    public float Jitter;
    //public Mesh particleMesh;
    //public Material particleMaterial;
    public float4 SurfaceColor;
    public float4 InteriorColor;
    public float4 ExteriorColor;
    public float ExteriorColorDist;
    public float InteriorColorDist;
    public float ColorStiffness;
}