using System;
using Unity.Entities;

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