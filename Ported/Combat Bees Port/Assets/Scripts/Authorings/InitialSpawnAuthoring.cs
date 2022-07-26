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

        public int beeCount;
        public int foodCount;

        public float3 yellowBase;
        public float3 blueBase;
        public float3 mapCenter;
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
                beeCount = authoring.beeCount,
                foodCount = authoring.foodCount,
                yellowBase = authoring.yellowBase,
                blueBase = authoring.blueBase,
                mapCenter = authoring.mapCenter
            });
        }
    }
}
