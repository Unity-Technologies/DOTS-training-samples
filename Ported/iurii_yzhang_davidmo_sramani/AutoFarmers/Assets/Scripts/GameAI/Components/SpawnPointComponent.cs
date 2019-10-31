using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace GameAI
{
    [Serializable]
    public struct SpawnPointComponent : IComponentData
    {
        public int2 MapSpawnPosition;
    }
}
