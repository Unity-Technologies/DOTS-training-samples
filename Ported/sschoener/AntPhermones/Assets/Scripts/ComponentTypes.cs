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

    public bool Equals(RenderData other)
    {
        return Equals(Mesh, other.Mesh) && Equals(Material, other.Material) && ShadowCastingMode == other.ShadowCastingMode && ReceiveShadows == other.ReceiveShadows;
    }
    
    public override bool Equals(object obj)
    {
        return obj is RenderData other && Equals(other);
    }

    public override int GetHashCode()
    {
        unchecked
        {
            var hashCode = (Mesh != null ? Mesh.GetHashCode() : 0);
            hashCode = (hashCode * 397) ^ (Material != null ? Material.GetHashCode() : 0);
            hashCode = (hashCode * 397) ^ (int)ShadowCastingMode;
            hashCode = (hashCode * 397) ^ ReceiveShadows.GetHashCode();
            return hashCode;
        }
    }
}

struct PheromoneRenderData : ISharedComponentData, IEquatable<PheromoneRenderData>
{
    public MeshRenderer Renderer;
    public Material Material;

    public bool Equals(PheromoneRenderData other)
    {
        return Equals(Renderer, other.Renderer) && Equals(Material, other.Material);
    }
    
    public override bool Equals(object obj)
    {
        return obj is PheromoneRenderData other && Equals(other);
    }

    public override int GetHashCode()
    {
        unchecked
        {
            return ((Renderer != null ? Renderer.GetHashCode() : 0) * 397) ^ (Material != null ? Material.GetHashCode() : 0);
        }
    }
}

public struct UninitializedTagComponent : IComponentData { }

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

public struct AntSpawnComponent : IComponentData
{
    public int Amount;
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
    public BlobArray<int> ObstacleBucketIndices;

    public bool HasObstacle(float2 pos)
    {
        TryGetObstacles(pos, out int offset, out int length);
        return length > 0;
    }

    
    public void TryGetObstacles(float2 pos, out int offset, out int length)
    {
        var c = (int2) (pos / MapSize * BucketResolution);
        if (math.any(c < 0) || math.any(c >= BucketResolution))
        {
            offset = length = 0;
            return;
        }
        int idx = c.y * BucketResolution + c.x;
        int next = idx == ObstacleBucketIndices.Length - 1 ? Obstacles.Length : ObstacleBucketIndices[idx + 1];
        offset = ObstacleBucketIndices[idx];
        length = next - offset;
    }
}