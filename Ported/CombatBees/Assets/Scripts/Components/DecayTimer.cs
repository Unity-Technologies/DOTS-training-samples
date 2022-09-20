using Unity.Entities;

struct DecayTimer : IComponentData, IEnableableComponent
{
    public float Value;
}