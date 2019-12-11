using Unity.Entities;
using Unity.Mathematics;

public struct HoldTargetState : IComponentData
{
    public float3 HeldTargetOffset;
}