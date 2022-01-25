using Unity.Entities;
using Unity.Mathematics;

public struct AnchorPoint : IComponentData
{
}

public struct Point : IComponentData
{
}

public struct PointOldTranslation : IComponentData
{
    public float3 Value;
}