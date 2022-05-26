using Unity.Entities;

[GenerateAuthoringComponent]
public struct MaxPossibleSpeedComponent : IComponentData
{
    public float MaxSpeed;
}
