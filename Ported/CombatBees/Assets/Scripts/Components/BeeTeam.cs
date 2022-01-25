using Unity.Entities;

public enum TeamValue
{
    Blue,
    Yellow
}



[GenerateAuthoringComponent]
public struct BeeTeam : IComponentData
{
    public TeamValue Value;
}