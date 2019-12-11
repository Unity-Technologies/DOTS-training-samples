using Unity.Entities;
using Unity.Mathematics;

public struct ConstrainedPoint : IComponentData
{
    float3 position;
    int neightbours;
}