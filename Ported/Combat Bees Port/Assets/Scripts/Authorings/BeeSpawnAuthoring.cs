using Components;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Authorings
{
    class BeeSpawnAuthoring : UnityEngine.MonoBehaviour
    {
        public GameObject yellowBeePrefab;
        public GameObject blueBeePrefab;
        public GameObject foodPrefab;

        public int beeCount;
        public int foodCount;
    }

    class BeeSpawnBaker : Baker<BeeSpawnAuthoring>
    {
        public override void Bake(BeeSpawnAuthoring authoring)
        {
            AddComponent(new BeeSpawnData
            {
                yellowBeePrefab = GetEntity(authoring.yellowBeePrefab),
                blueBeePrefab = GetEntity(authoring.blueBeePrefab),
                foodPrefab = GetEntity(authoring.foodPrefab),
                beeCount = authoring.beeCount,
                foodCount = authoring.foodCount,
            });
        }
    }
}
