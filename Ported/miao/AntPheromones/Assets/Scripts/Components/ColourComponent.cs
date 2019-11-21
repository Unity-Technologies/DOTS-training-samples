using Unity.Entities;
using UnityEngine;

namespace AntPheromones_ECS
{
    public struct ColourComponent : IComponentData
    {
        public Color Value;
    }
}