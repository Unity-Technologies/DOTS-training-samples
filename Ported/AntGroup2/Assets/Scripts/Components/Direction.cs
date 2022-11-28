using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;


struct PheromoneDirection : IComponentData
{
    public float2 Direction;
}

struct TargetDirection : IComponentData
{
    public float2 Direction;
}

struct WallDirection : IComponentData
{
    public float2 Direction;
}