using Unity.Entities;
using Unity.Mathematics;

public struct RelocationPosition : IComponentData
{
    public float positionAlongSpline; // 0-1
}
