using System;
using Unity.Entities;

namespace src
{
    [Serializable]
    public struct FollowTranslation : IComponentData
    {
        public Entity Target;
        public float Distance;
    }
}