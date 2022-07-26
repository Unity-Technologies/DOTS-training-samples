using Unity.Entities;
using Unity.Mathematics;

// An empty component is called a "tag component".
struct LineEndPosition : IComponentData
{
    public float2 Value;
}