using Unity.Entities;

struct StackIndex : IComponentData, IEnableableComponent
{
    public int Value;
}