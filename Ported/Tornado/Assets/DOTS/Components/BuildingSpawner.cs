using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

namespace Dots
{
    public class BuildingSpawner : IComponentData
    {
        public Mesh mesh;
        public Material material;

        public int buildingCount;
        public int minHeight;
        public int maxHeight;
        public int debrisCount;
        public float thicknessMin;
        public float thicknessMax;
    }
}
