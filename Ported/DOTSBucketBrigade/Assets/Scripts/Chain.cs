using Unity.Entities;
using Unity.Mathematics;

public struct Chain : IComponentData
{
    public float3 ChainStartPosition;
    public float3 ChainEndPosition;
}
