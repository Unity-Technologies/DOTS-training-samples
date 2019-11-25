using Unity.Entities;
using UnityEngine;

namespace AntPheromones_ECS
{
    public struct Colour : IComponentData
    {
        public Color Value;
    }
}