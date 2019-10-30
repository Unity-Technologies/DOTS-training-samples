using Unity.Entities;
using UnityEngine;

namespace GameAI
{
    public struct FarmerComponent : IComponentData
    {
        public Vector2Int Position;
    };
}