using Unity.Entities;

public struct DependentEntity : IComponentData
{
    public Entity Dependent;
}
