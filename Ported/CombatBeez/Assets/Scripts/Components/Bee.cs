using Unity.Entities;
using Unity.Mathematics;

struct Bee : IComponentData
{
    public float3 OcillateOffset;
    public float3 Target;
}
