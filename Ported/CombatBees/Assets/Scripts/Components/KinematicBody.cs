using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Components
{
    public struct KinematicBody : IComponentData
    {
        public float3 Velocity;
        public float Height;
        public float3 LandPosition;
    }
}