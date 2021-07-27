using Unity.Entities;
using UnityRangeAttribute = UnityEngine.RangeAttribute;

namespace DOTSRATS
{
    [GenerateAuthoringComponent]
    public struct RatSpawner : IComponentData
    {
        public Entity ratPrefab;
        public int maxRats;
        [UnityRange(0, 1)] public float spawnRate;
    }
}
