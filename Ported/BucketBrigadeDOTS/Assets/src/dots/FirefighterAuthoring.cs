using Unity.Entities;
using Unity.Mathematics;

public struct Firefighter : IComponentData
{
}

public struct FirefighterNext : IComponentData
{
    public Entity Value;
}

public struct WaterBucketID : IComponentData
{
    public Entity Value;
}

public struct FirefighterFullTag : IComponentData
{
}

public struct FirefighterEmptyTag : IComponentData
{
}

public struct Target : IComponentData
{
    public float2 Value;
}

public struct RelayReturn : IComponentData
{
    public float2 Value;
}

public struct FirefighterPositionInLine : IComponentData
{
    public float Value;
}
