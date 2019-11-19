using Unity.Entities;
using UnityEngine;

namespace AntPheromones_ECS
{
    public struct ColourDisplay : IComponentData
    {
        public Color Value;
    }
}