using Unity.Entities;
using UnityEngine;

namespace AntPheromones_ECS
{
    public struct RenderData : ISharedComponentData
    {
        public Material Material;
        public Mesh Mesh;
    }
}