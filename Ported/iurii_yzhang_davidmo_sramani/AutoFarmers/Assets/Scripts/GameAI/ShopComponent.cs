using Unity.Entities;
using UnityEngine;

namespace GameAI
{
    public struct ShopComponent : IComponentData
    {
        public Vector2Int Position;
    };
}