using Unity.Entities;
using Unity.Mathematics;

public struct ReachForTargetState : IComponentData
{
    public Entity TargetEntity;
    public float TargetSize;
    public float3 HandTarget;
}