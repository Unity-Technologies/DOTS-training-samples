using Unity.Entities;
using UnityEngine;

namespace AntPheromones_ECS
{
    public struct Position : IComponentData
    {
        public Vector2 Value;
    }
}