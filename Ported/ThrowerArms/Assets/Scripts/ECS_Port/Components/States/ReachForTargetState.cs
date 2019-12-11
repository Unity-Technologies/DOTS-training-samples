using Unity.Entities;
using Unity.Mathematics;

public struct ReachForTargetState : IComponentData
{
    public float ReachTimer;
    public Entity TargetEntity;
    public float TargetSize;
    public float3 FullyReachedOutHandTranslation;
}