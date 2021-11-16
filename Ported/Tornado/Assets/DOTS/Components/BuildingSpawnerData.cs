using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace Dots
{
    [Serializable]
    public struct BuildingSpawnerData : IComponentData
    {
        public Entity BeamPrefab;
        public int BuildingCount;
        public bool UseBeamGroups;
    }
}