using System;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public struct ParticlePosition : IComponentData
{
    public float3 value;
}
public struct ParticleVelocity : IComponentData
{
    public float3 value;
}
public struct ParticleColor : IComponentData
{
    public float4 value;
}
public struct ParticleMesh : ISharedComponentData, IEquatable<ParticleMesh>
{
    public Mesh Mesh;
    public Material Material;

    public bool Equals(ParticleMesh other)
    {
        return Equals(Mesh, other.Mesh) && Equals(Material, other.Material);
    }

    public override bool Equals(object obj)
    {
        return obj is ParticleMesh other && Equals(other);
    }

    public override int GetHashCode()
    {
        unchecked
        {
            var hashCode = (Mesh != null ? Mesh.GetHashCode() : 0);
            hashCode = (hashCode * 397) ^ (Material != null ? Material.GetHashCode() : 0);
            return hashCode;
        }
    }

}
