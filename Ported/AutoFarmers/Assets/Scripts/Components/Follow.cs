using Unity.Entities;
using Unity.Mathematics;

struct Follow : IComponentData
{
    public float3 Offset;
    public Entity EntityToFollow;
}
