using Unity.Entities;
using Unity.Mathematics;

public struct AttractionRepulsion : IComponentData
{
    public float3 AttractionPos;
    public float3 RepulsionPos;
}