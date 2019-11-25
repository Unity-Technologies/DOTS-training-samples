using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace AntPheromones_ECS
{
    public struct Position : IComponentData
    {
        public float2 Value;
    }
}