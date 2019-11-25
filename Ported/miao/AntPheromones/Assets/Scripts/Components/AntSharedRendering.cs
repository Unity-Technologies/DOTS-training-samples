using System;
using Unity.Entities;
using UnityEngine;

namespace AntPheromones_ECS
{
    public struct AntSharedRendering : ISharedComponentData, IEquatable<AntSharedRendering>
    {
        public Material Material;
        public Mesh Mesh;

        public bool Equals(AntSharedRendering other)
        {
            return Equals(this.Material, other.Material) && Equals(this.Mesh, other.Mesh);
        }

        public override bool Equals(object obj)
        {
            return obj is AntSharedRendering other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (this.Material.GetHashCode()* 397) ^ (this.Mesh.GetHashCode());
            }
        }
    }
}