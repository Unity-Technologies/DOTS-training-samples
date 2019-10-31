using Unity.Entities;

public struct FixedDistanceConstraint : IComponentData
{
    public float distance;
    public int pointId1;
    public int pointId2;
}