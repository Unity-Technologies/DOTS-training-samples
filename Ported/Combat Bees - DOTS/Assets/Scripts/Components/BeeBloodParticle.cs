using Unity.Entities;

[GenerateAuthoringComponent]
public struct BeeBloodParticle : IComponentData
{
    public float timeToLive;
    public float steps;
}
