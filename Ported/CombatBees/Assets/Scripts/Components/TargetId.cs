using Unity.Entities;

struct TargetId : IComponentData, IEnableableComponent
{
    public Entity Value;
}