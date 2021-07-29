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
        [HideInInspector] public int maxAnimals;
        public int2 initialSpeed;
        [HideInInspector] public Unity.Mathematics.Random random;
        [HideInInspector] public float timeToNextSpawn;
        [UnityRange(0, 1)] public float spawnRate;
    }
}

