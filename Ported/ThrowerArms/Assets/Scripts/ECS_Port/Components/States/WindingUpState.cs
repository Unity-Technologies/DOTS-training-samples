using Unity.Entities;

public struct WindingUpState : IComponentData
{
    public float WindupTimer;
    public Entity AimedTargetEntity;
    public Entity HeldEntity;
}