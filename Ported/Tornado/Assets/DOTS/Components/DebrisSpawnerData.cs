using System;
using Unity.Entities;

namespace Dots
{
    [Serializable]
    public struct DebrisSpawnerData : IComponentData
    {
        public Entity debrisPrefab;
        public float debrisCount;
        public float initRange;
        public float height;
    }
}