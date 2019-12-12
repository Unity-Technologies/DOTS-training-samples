using System;
using Unity.Entities;
using Unity.Mathematics;

namespace src
{
    [Serializable]
    public struct TargetSpeed : IComponentData
    {
        public float Value;

        public float3 LastPosition;
    }
}