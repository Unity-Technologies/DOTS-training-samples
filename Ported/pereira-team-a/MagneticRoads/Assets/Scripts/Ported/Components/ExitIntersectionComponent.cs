using Unity.Entities;

public struct ExitIntersectionComponent2 : IComponentData
{
    // If this a temporary spline, what is the next spline it will connect to
    public int TargetSplineId;
}