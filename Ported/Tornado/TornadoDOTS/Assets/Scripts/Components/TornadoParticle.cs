using Unity.Entities;

[GenerateAuthoringComponent]
public struct TornadoParticle : IComponentData
{
    public float SpinRate;
    public float UpwardSpeed;
    public float RadiusMult;
}
