using Unity.Entities;

public struct Team : IComponentData
{
    public TeamName Value;
    public int IndexInTeam;
}

public enum TeamName
{
    Undefined,
    A,
    B
}