using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace GameAI
{
    public struct TargetTag : IComponentData
    {
        public Translation TargetPosition;
    }
    
    public struct FarmerComponent : IComponentData
    {
        public int2 Position;
    };
}