using Components;
using Unity.Entities;
using Unity.Mathematics;

namespace Authorings
{
    class InitialSpawnAuthoring : UnityEngine.MonoBehaviour
    {
        public Entity yellowBeePrefab;
        public Entity blueBeePrefab;
        public Entity foodPrefab;


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
                yellowBeePrefab = authoring.yellowBeePrefab,
                blueBeePrefab = authoring.blueBeePrefab,
                foodPrefab = authoring.foodPrefab,
                beeCount = authoring.beeCount,
                foodCount = authoring.foodCount,
                yellowBase = authoring.yellowBase,
                blueBase = authoring.blueBase,
                mapCenter = authoring.mapCenter
            });
        }
    }
}
