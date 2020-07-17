using Unity.Entities;
using Unity.Mathematics;

[GenerateAuthoringComponent]
public struct Bounds : IComponentData
{
    public float3 Center;
    public float3 Extents;

    public float3 Size => Extents * 2;
    public float3 Min => Center - Extents;
    public float3 Max => Center + Extents;

    public float Floor => Center.y - Extents.y;

    public bool Intersects(float3 position, bool ignoreX = false, bool ignoreY = false, bool ignoreZ = false)
    {
        var min = Min;
        var max = Max; 
        return (ignoreX || (position.x >= min.x && position.x < max.x)) &&
                (ignoreY || (position.y >= min.y && position.y < max.y)) &&
                (ignoreZ || (position.z >= min.z && position.z < max.z));
    }
}
