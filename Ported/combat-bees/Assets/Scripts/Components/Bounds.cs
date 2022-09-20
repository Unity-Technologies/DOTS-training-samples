using Unity.Entities;
using Unity.Mathematics;

public struct Bounds : IComponentData
{
    public AABB Value;
}
