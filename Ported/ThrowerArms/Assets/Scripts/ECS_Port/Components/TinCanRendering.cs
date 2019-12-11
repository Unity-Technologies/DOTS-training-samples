using System;
using Unity.Entities;
using UnityEngine;

public struct TinCanRendering : ISharedComponentData, IEquatable<TinCanRendering>
{
    public Material Material;
    public Mesh Mesh;
    public float Height;
    public float Diameter;

    public bool Equals(TinCanRendering other)
    {
        return Equals(Material, other.Material)
               && Equals(Mesh, other.Mesh)
               && Height.Equals(other.Height) 
               && Diameter.Equals(other.Diameter);
    }

    public override bool Equals(object obj)
    {
        return obj is TinCanRendering other && Equals(other);
    }

    public override int GetHashCode()
    {
        unchecked
        {
            var hashCode = Material != null ? Material.GetHashCode() : 0;
            hashCode = (hashCode * 397) ^ (Mesh != null ? Mesh.GetHashCode() : 0);
            hashCode = (hashCode * 397) ^ Height.GetHashCode();
            hashCode = (hashCode * 397) ^ Diameter.GetHashCode();
            return hashCode;
        }
    }
}