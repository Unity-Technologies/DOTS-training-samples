using Combatbees.Testing.Elfi;
using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Random = Unity.Mathematics.Random;

namespace Combatbees.Testing.Elfi
{
    public struct EResourceComponent : IComponentData
    {
        public Entity resourcePrefab;

        public float gridX;
        public float gridZ;
        public int startResourceCount;
        public Random random;
        public int spawnedResources;

    }
}