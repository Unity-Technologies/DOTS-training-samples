using Unity.Entities;

public struct Constraint : IComponentData
{
    public Node pointA;
    public Node pointB;
    public float distance;
}