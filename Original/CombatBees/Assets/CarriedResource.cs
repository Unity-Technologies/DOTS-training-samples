using Unity.Entities; 

[GenerateAuthoringComponent]
public struct CarriedResource : IComponentData
{
    public Entity Value;
}