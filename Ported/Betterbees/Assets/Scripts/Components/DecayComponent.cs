using Unity.Entities;

public struct DecayComponent : IComponentData, IEnableableComponent
{
    public float DecayRate;
}
