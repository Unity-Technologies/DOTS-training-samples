using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace GameAI
{
    public struct RockComponent : IComponentData
    {
        public int2 Size;
    };
}