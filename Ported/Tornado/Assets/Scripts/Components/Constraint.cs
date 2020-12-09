using Unity.Entities;
using Unity.Mathematics;

public struct Constraint : IBufferElementData
{
    public Entity pointA;
    public Entity pointB;
    public float distance;
}