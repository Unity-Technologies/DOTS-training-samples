using System;
using Unity.Entities;
using UnityEngine;

public struct ArmRendering : ISharedComponentData, IEquatable<ArmRendering>
{
    public Material Material;
    public Mesh Mesh;

    public bool Equals(ArmRendering other)
    {
        return Equals(Material, other.Material) && Equals(Mesh, other.Mesh);
    }

    public override bool Equals(object obj)
    {
        return obj is ArmRendering other && Equals(other);
    }

    public override int GetHashCode()
    {
        unchecked
        {
            return ((Material != null ? Material.GetHashCode() : 0) * 397) ^ (Mesh != null ? Mesh.GetHashCode() : 0);
        }
    }
}