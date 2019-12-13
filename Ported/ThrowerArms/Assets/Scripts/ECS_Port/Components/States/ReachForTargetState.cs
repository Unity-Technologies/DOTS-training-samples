using Unity.Entities;
using Unity.Mathematics;

public struct ReachForTargetState : IComponentData
{
    public Entity TargetEntity;
    public float3 HandTarget;
}