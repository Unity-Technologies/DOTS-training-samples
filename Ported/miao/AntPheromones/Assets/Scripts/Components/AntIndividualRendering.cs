using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace AntPheromones_ECS
{
    public struct AntIndividualRendering : IComponentData
    {
        public Color CarryColour;
        public Color SearchColour;
        public float3 Scale;
    }
}