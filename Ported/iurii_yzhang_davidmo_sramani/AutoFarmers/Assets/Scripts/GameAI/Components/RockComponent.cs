using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace GameAI
{
    public struct RockComponent : IComponentData
    {
        public float Size;
        public int2 Position;
    };
}