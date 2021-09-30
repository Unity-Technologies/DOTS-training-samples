using Unity.Entities;
using Unity.Mathematics;

[GenerateAuthoringComponent]
public struct BeeMovement : IComponentData
{
    public float CurrentVelocity;
    public float TimeToChangeVelocity;
}