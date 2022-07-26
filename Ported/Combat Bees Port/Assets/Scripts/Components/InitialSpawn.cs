using System;
using Unity.Entities;
using Unity.Mathematics;

namespace Components
{
    struct InitialSpawn : IComponentData
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
}
