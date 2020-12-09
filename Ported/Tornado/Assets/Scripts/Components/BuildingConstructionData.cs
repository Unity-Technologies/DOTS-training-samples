using Unity.Entities;
using Unity.Mathematics;

public struct BuildingConstructionData : IComponentData
{
    public int height;
    public float3 position;
    public float spacing;
}