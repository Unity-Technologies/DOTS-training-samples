using System;
using Unity.Entities;
using Unity.Mathematics;

public struct SplineComponent : IComponentData
{
    public SplineBufferElementData SplineBufferElementData;
    public bool IsInsideIntersection;
}