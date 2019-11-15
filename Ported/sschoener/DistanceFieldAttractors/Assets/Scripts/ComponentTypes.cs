using System;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Systems
{
    public struct UninitializedTagComponent : IComponentData { }

    public struct VelocityComponent : IComponentData
    {
        public float3 Value;
    }

    public struct PositionComponent : IComponentData
    {
        public float3 Value;
    }

    public struct LocalToWorldComponent : IComponentData
    {
        public float4x4 Value;
    }

    public struct PositionInDistanceFieldComponent : IComponentData
    {
        public float3 Normal;
        public float Distance;
    }

    public struct RenderColorComponent : IComponentData
    {
        public Color Value;
    }

    public struct ParticleSetupComponent : ISharedComponentData, IEquatable<ParticleSetupComponent>
    {
        public Mesh Mesh;
        public Material Material;

        public Color SurfaceColor;
        public Color InteriorColor;
        public Color ExteriorColor;

        public float SpeedStretch;
        public float Jitter;
        public float Attraction;

        public float ExteriorColorDist;
        public float InteriorColorDist;
        public float ColorStiffness;

        public bool Equals(ParticleSetupComponent other)
        {
            return Equals(Mesh, other.Mesh) && Equals(Material, other.Material) &&
                SurfaceColor.Equals(other.SurfaceColor) && InteriorColor.Equals(other.InteriorColor) &&
                ExteriorColor.Equals(other.ExteriorColor) && SpeedStretch.Equals(other.SpeedStretch) &&
                Jitter.Equals(other.Jitter) && Attraction.Equals(other.Attraction) &&
                ExteriorColorDist.Equals(other.ExteriorColorDist) &&
                InteriorColorDist.Equals(other.InteriorColorDist) && ColorStiffness.Equals(other.ColorStiffness);
        }

        public override bool Equals(object obj)
        {
            return obj is ParticleSetupComponent other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (Mesh != null ? Mesh.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Material != null ? Material.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ SurfaceColor.GetHashCode();
                hashCode = (hashCode * 397) ^ InteriorColor.GetHashCode();
                hashCode = (hashCode * 397) ^ ExteriorColor.GetHashCode();
                hashCode = (hashCode * 397) ^ SpeedStretch.GetHashCode();
                hashCode = (hashCode * 397) ^ Jitter.GetHashCode();
                hashCode = (hashCode * 397) ^ Attraction.GetHashCode();
                hashCode = (hashCode * 397) ^ ExteriorColorDist.GetHashCode();
                hashCode = (hashCode * 397) ^ InteriorColorDist.GetHashCode();
                hashCode = (hashCode * 397) ^ ColorStiffness.GetHashCode();
                return hashCode;
            }
        }
    }

    public struct DistanceFieldComponent : IComponentData
    {
        public DistanceFieldModel ModelType;
        public float TimeToSwitch;
    }

    public struct SpawnParticleComponent : IComponentData
    {
        public int Count;
    }
}
