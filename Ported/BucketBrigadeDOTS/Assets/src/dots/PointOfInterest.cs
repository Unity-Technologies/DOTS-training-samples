using Unity.Entities;
using Unity.Mathematics;


public struct PointOfInterestRequest : IComponentData
{
    public float2 POIReferencePosition;
}

public struct PointOfInterestEvaluated : IComponentData
{
    public float2 POIPoisition;
}