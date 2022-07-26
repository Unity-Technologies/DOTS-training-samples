﻿using System;
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
    }
}
