using Unity.Entities;
using Unity.Mathematics;
struct Velocity : IComponentData
{
    // Should be Value by convention.
    public float3 velocity;
}
