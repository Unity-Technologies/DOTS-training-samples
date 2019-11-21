using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace AntPheromones_ECS
{
    public struct PositionComponent : IComponentData
    {
        public float2 Value;
    }
}