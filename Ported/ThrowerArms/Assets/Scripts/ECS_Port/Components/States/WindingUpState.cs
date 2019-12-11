using Unity.Entities;
using Unity.Mathematics;

public struct WindingUpState : IComponentData
{
    public float WindupTimer;
    public float3 HandTarget;
    public Entity AimedTargetEntity;
    public Entity HeldEntity;
    public float3 FullyWoundUpHandTranslation;
}