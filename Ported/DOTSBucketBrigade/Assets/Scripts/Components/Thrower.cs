using Unity.Entities;
using Unity.Mathematics;

public struct Thrower : IComponentData
{
    public int2 Coord;
    public int2 TargetCoord;
    public float2 GridPosition;
};
