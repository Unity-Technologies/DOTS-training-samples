using Unity.Entities;

[GenerateAuthoringComponent]
struct Bounciness : IComponentData
{
    public float Value;
}