using Unity.Entities;

[GenerateAuthoringComponent]
public struct ParticleComponent : IComponentData
{
    public ParticleType Type;
}
