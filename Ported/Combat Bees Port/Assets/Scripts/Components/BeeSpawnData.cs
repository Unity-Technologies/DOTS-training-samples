using System;
using Unity.Entities;
using Unity.Mathematics;

namespace Components
{
    struct BeeSpawnData : IComponentData
    {
        public Entity yellowBeePrefab;
        public Entity blueBeePrefab;

        public int beeCount;
    }
}
