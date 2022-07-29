using Unity.Entities;
using Unity.Mathematics;

// An empty component is called a "tag component".
struct Position : IComponentData
{
    public float2 Start;
    public float2 End;
}