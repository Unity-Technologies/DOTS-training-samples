﻿using Unity.Entities;
using Unity.Mathematics;

enum BeeTeam
{
    Blue,
    Yellow
}

struct BeePrototype : IComponentData
{
    public BeeTeam Hive;
    
    public float3 SpawnPosition;
}