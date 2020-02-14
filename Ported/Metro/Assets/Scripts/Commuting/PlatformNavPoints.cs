using Unity.Entities;
using Unity.Mathematics;

public struct PlatformNavPoints : IComponentData
{
    public float3 backDown;
    public float3 backUp;
    public float3 frontDown;
    public float3 frontUp;
}
