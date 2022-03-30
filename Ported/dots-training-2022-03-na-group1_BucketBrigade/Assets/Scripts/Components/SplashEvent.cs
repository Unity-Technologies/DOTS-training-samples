using Unity.Entities;
using Unity.Mathematics;

public struct SplashEvent : IComponentData
{
    public float3 splashWorldPositionn;
    public int fireTileIndex;
}
