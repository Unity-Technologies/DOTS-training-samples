using Unity.Entities;
using Unity.Mathematics;

// An empty component is called a "tag component".
struct LineStartPosition : IComponentData
{
    public float2 Value;
}