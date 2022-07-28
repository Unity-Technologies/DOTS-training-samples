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
        public Entity spawnFlashPrefab;
        public Entity bloodPrefab;

        public int beeCount;
        public int foodCount;

        public int beePulseSpawnCount;
    }
}
