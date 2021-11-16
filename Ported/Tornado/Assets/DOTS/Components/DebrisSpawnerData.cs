using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

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