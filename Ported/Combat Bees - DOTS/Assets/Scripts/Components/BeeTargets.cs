using Unity.Entities;
using Unity.Mathematics;

public struct BeeTargets : IComponentData
{
    public Entity ResourceTarget;
    public Entity EnemyTarget;
    public float3 HomePosition;
    public float3 CurrentTargetPosition;
}