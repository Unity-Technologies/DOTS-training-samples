using Unity.Entities;
using Unity.Mathematics;

public struct Constraint : IComponentData
{
    public Node pointA;
    public Node pointB;
    public float distance;
}