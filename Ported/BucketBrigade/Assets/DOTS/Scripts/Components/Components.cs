using Unity.Entities;
using Unity.Collections;
using Unity.Mathematics;
using Unity.Transforms;

public struct TargetPosition : IComponentData
{
    public float2 Value;
}
