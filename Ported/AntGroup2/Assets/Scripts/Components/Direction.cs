using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;

struct CurrentDirection : IComponentData
{
    public float Angle;
}

struct PreviousDirection : IComponentData
{
    public float Angle;
}

struct PheromoneDirection : IComponentData
{
    public float Angle;
}

struct TargetDirection : IComponentData
{
    public float Angle;
}

struct WallDirection : IComponentData
{
    public float Angle;
    public int WallBounceDirection; // 0 == no bounce, 1 bounce outside of the circle, -1 inside of the circle
}
