using Unity.Entities;

public struct Constraint : IBufferElementData
{
    public Entity pointA;
    public Entity pointB;
    public float distance;
}