using Unity.Entities;
using UnityEngine;

namespace GameAI
{
    public struct RockComponent : IComponentData
    {
        public float Size;
        public Vector2Int Position;
    };
}