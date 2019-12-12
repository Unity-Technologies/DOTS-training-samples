using Unity.Entities;

public struct ParticleSettings : IComponentData
{
    public Entity prefab;
    public float spinRate;
    public float upwardSpeed;
}