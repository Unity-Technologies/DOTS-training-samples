using Unity.Entities; 

[GenerateAuthoringComponent]
public struct CarriedFood : IComponentData
{
    public Entity Value;
}