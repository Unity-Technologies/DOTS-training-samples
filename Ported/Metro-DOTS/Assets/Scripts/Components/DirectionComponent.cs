using UnityEngine;
using Unity.Entities;

namespace Components
{
    public struct DirectionComponent : IComponentData
    {
        public Vector3 Direction;
    }
}
