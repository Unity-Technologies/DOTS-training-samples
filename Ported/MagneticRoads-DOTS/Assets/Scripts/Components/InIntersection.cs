using Unity.Entities;

public struct InIntersection : IComponentData
{
    public int nextSpline;
    public Entity intersection;
}
