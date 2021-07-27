using Unity.Entities;
using Unity.Mathematics;
using UnityRangeAttribute = UnityEngine.RangeAttribute;

namespace DOTSRATS
{
    [GenerateAuthoringComponent]
    public struct CatSpawner : IComponentData
    {
        public Entity catPrefab;
        public int maxCats;
        [UnityRange(0, 1)] public float spawnRate;
        public float3 spawnPointOne;
        public float3 spawnPointTwo;
    }
}
