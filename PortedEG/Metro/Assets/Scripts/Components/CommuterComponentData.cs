using Unity.Entities;
using Unity.Mathematics;

// An empty component is called a "tag component".
struct CommuterTag : IComponentData
{
}

struct CommuterSpeed : IComponentData
{
    public float3 Value;
}
