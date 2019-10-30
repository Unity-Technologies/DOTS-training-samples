using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace GameAI
{
    public struct FarmerComponent : IComponentData
    {
        public int2 Position;
    };
}