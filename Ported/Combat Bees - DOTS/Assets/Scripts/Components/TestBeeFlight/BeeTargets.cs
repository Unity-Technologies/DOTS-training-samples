using Unity.Entities;
using Unity.Mathematics;

public struct BeeTargets : IComponentData
{
    public float3 LeftTarget;
    public float3 RightTarget;
    public float3 CurrentTarget;
    public float TargetWithinReach;
}
