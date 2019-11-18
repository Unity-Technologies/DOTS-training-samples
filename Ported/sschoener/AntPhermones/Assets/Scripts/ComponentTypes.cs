using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public struct HasResourcesTagComponent : IComponentData { }

public struct HasResourcesComponent : IComponentData
{
    public bool Value;
}

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

public struct LocalToWorldComponent : IComponentData
{
    public float4x4 Value;
}

public struct BrightnessComponent : IComponentData
{
    public float Value;
}

public struct RenderColorComponent : IComponentData
{
    public Color Value;
}

public struct MapSettingsComponent : IComponentData
{
    public int MapSize;


    public float TrailDecay;
    public float TrailAdd;
    public float MaxSpeed;

    public float ResourcePosition;
    public float2 ColonyPosition;
    public float InwardStrength;
    public float OutwardStrength;


    public float ObstacleRadius;    
    public BlobAssetReference<ObstacleList> Obstacles;
}

public struct PheromoneMapComponent : IComponentData
{
    public BlobAssetReference<PheromoneMap> PheromoneMap;
}

public struct PheromoneMap
{
    public BlobArray<float> Map;
}

public struct ObstacleList
{
    public BlobArray<float2> Obstacles;
}