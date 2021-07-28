using Unity.Entities;
using Unity.Mathematics;
using UnityRangeAttribute = UnityEngine.RangeAttribute;

namespace DOTSRATS
{
    [GenerateAuthoringComponent]
    public struct AnimalSpawner : IComponentData
    {
        public Entity animalPrefab;
        public int maxAnimals;
        [UnityRange(0, 1)] public float spawnRate;
    }
}

