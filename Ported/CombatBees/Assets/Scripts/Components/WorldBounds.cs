using Unity.Entities;
using Unity.Mathematics;

[GenerateAuthoringComponent]
public struct WorldBounds : IComponentData
{
    public MinMaxAABB AABB;
}
