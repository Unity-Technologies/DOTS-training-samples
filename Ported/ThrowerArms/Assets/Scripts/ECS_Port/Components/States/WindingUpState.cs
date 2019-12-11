using Unity.Entities;
using Unity.Mathematics;

public struct WindingUpState : IComponentData
{
    public float WindupTimer;
    public Entity AimedTargetEntity;
    public Entity HeldEntity;
    public float3 FullyWoundUpHandTranslation;
}