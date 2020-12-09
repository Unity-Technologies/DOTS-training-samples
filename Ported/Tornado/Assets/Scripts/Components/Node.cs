using Unity.Entities;
using Unity.Mathematics;

public struct Node : IComponentData
{
    public bool anchor;
    public int neighborCount;
    public float3 oldPosition;
}