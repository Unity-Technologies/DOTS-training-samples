using Unity.Entities;

public struct MoveTowardBucket : IComponentData
{
    public Entity Target;
}

public struct MoveTowardFire : IComponentData
{
    public Entity Target;
}

public struct MoveTowardWater : IComponentData
{
    public Entity Target;
}

public struct MoveTowardFiller : IComponentData
{
    public Entity Target;
}