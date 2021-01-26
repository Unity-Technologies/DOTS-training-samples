using Unity.Entities; 

[GenerateAuthoringComponent]
public struct BeeTag : IComponentData
{
    public int teamId;
}