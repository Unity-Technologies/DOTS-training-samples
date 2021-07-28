using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using UnityRangeAttribute = UnityEngine.RangeAttribute;

namespace DOTSRATS
{
    [GenerateAuthoringComponent]
    public struct AnimalSpawner : IComponentData
    {
        public Entity animalPrefab;
        public int maxAnimals;
        public int2 initialSpeed;
        [HideInInspector] public uint randomSeed;
        [HideInInspector] public float timeToNextSpawn;
        [UnityRange(0, 1)] public float spawnRate;
    }
}

