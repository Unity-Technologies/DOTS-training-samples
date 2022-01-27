using Unity.Entities;

[GenerateAuthoringComponent]
public struct Passenger : IComponentData
{
    public Entity Body;
}
