using System;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Rendering;

struct RenderData : ISharedComponentData, IEquatable<RenderData>
{
    public Mesh Mesh;
    public Material Material;
    public ShadowCastingMode ShadowCastingMode;
    public bool ReceiveShadows;

    public bool Equals(RenderData other) =>
        ReceiveShadows == other.ReceiveShadows &&
        ShadowCastingMode == other.ShadowCastingMode &&
        Material == other.Material &&
        Mesh == other.Mesh;
}

struct PheromoneRenderData : ISharedComponentData, IEquatable<PheromoneRenderData>
{
    public MeshRenderer Renderer;
    public Material Material;

    public bool Equals(PheromoneRenderData other) => Renderer == other.Renderer && Material == other.Material;
}

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

public struct AntRenderSettingsComponent : IComponentData
{
    public Color CarrierColor;
    public Color SearcherColor;
    public float3 Scale;
}

public struct AntSteeringSettingsComponent : IComponentData
{
    public float RandomSteerStrength;
    public float WallSteerStrength;
    public float PheromoneSteerStrength;
    public float TargetSteerStrength;
    public float InwardSteerStrength;
    public float OutwardSteerStrength;
    
    public float MaxSpeed;
    public float Acceleration;
}

public struct MapSettingsComponent : IComponentData
{
    public int MapSize;
    
    public float TrailDecay;
    public float TrailAdd;

    public float2 ResourcePosition;
    public float2 ColonyPosition;
    
    public float ObstacleRadius;    
    public BlobAssetReference<ObstacleData> Obstacles;
}

public struct PheromoneBuffer : IBufferElementData
{
    public float Value;
    
    public static implicit operator PheromoneBuffer(float v) => new PheromoneBuffer { Value = v };
    public static implicit operator float(PheromoneBuffer p) => p.Value;
}

public struct ObstacleData
{
    public int BucketResolution;
    public int MapSize;
    public BlobArray<float2> Obstacles;
    public BlobArray<bool> ObstacleMap;

    public bool HasObstacle(float2 pos)
    {
        var c = pos / MapSize * BucketResolution;
        if (math.any(c < 0) || math.any(c > MapSize))
            return false;
        return ObstacleMap[(int)pos.y * MapSize + (int)pos.x];
    }
}