using Components;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Authorings
{
    class InitialSpawnAuthoring : UnityEngine.MonoBehaviour
    {
        public GameObject yellowBeePrefab;
        public GameObject blueBeePrefab;
        public GameObject foodPrefab;
        public GameObject spawnFlashPrefab;

        public int beeCount;
        public int foodCount;
        public int beePulseSpawnCount;
    }

    class InitialSpawnBaker : Baker<InitialSpawnAuthoring>
    {
        public override void Bake(InitialSpawnAuthoring authoring)
        {
            AddComponent(new InitialSpawn
            {
                yellowBeePrefab = GetEntity(authoring.yellowBeePrefab),
                blueBeePrefab = GetEntity(authoring.blueBeePrefab),
                foodPrefab = GetEntity(authoring.foodPrefab),
                spawnFlashPrefab = GetEntity(authoring.spawnFlashPrefab),
                beeCount = authoring.beeCount,
                foodCount = authoring.foodCount,
                beePulseSpawnCount = authoring.beePulseSpawnCount
            });
        }
    }
}
