using System;
using Unity.Entities;

namespace Dots
{
    [Serializable]
    public struct BuildingSpawnerData : IComponentData
    {
        public Entity beamPrefab;
        public int buildingCount;
        public int buildingAreaSize;
        public int buildingMaxFloorCount;
        public int groundBeamsPointCount;
        public int groundBeamsAreaSize;
        public int groundBeamsCells;
    }
}