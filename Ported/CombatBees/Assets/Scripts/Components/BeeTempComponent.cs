using Unity.Entities;
using Unity.Mathematics;

struct BeeTempComponent : IComponentData
{
    public float3 Speed;
    public float3 Position;
}