using Unity.Entities;

public struct DistanceConstraint : IComponentData
{
    public float distance;
    public int pointId1;
    public int pointId2;
}