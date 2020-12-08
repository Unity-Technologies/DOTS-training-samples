using Unity.Entities;

public struct Constraint : IBufferElementData
{
    public Node pointA;
    public Node pointB;
    public float distance;
}