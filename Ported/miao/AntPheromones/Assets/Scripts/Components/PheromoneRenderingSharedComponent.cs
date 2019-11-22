using System;
using Unity.Entities;
using UnityEngine;

namespace AntPheromones_ECS
{
    public struct PheromoneRenderingSharedComponent : ISharedComponentData, IEquatable<PheromoneRenderingSharedComponent>
    {
        public MeshRenderer Renderer;
        public Material Material;

        public bool Equals(PheromoneRenderingSharedComponent other)
        {
            return Equals(this.Renderer, other.Renderer) && Equals(this.Material, other.Material);
        }

        public override bool Equals(object obj)
        {
            return obj is PheromoneRenderingSharedComponent other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (this.Renderer.GetHashCode() * 397) ^ (this.Material.GetHashCode());
            }
        }
    }
}