using Unity.Entities;

public struct Player : IComponentData
{
    public float BounceTime;
    public float CooldownTime;
    public float BallJumpRatio;
}