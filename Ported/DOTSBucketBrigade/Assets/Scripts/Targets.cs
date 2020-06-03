using Unity.Entities;
using Unity.Mathematics;

public struct TargetBucket : IComponentData
{
    public Entity Target;
}

public struct TargetWaterSource : IComponentData
{
    public Entity Target;
}

public struct TargetNextChain : IComponentData
{
    public Entity Target;
}

public struct TargetFire: IComponentData
{
    public int2 GridIndex;
}

public struct TargetPosition: IComponentData
{
    public float3 Target;
}

