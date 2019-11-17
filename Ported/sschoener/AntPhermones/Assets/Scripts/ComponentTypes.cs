using Unity.Entities;
using Unity.Mathematics;

public struct HasResourcesTagComponent : IComponentData { }

public struct FacingAngleComponent : IComponentData
{
    public float Value;
}

public struct PositionComponent : IComponentData
{
    public float2 Value;
}

public struct WallSteeringComponent : IComponentData
{
    public float Value;
}

public struct PheromoneSteeringComponent : IComponentData
{
    public float Value;
}

public struct SpeedComponent : IComponentData
{
    public float Value;
}

public struct VelocityComponent : IComponentData
{
    public float2 Value;
}

public struct TargetComponent : IComponentData
{
    public float2 Value;
}


public struct RenderColorComponent : IComponentData
{
    public float Brightness;
    public float InterpolatedValue;
}