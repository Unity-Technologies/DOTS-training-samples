using Unity.Entities;
using Unity.Mathematics;

// An empty component is called a "tag component".
struct CommuterPos : IComponentData
{
    float3 Value;
}
