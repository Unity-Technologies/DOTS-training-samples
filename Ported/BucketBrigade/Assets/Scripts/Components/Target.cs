using Unity.Entities;
using Unity.Mathematics;

// An empty component is called a "tag component".
struct Target : IComponentData
{
    public float2 Value;
}