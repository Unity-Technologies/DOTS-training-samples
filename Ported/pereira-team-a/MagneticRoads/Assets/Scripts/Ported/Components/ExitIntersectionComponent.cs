using Unity.Entities;

public struct ExitIntersectionComponent : IComponentData
{
    // If this a temporary spline, what is the next spline it will connect to
    public int TargetSplineId;
}