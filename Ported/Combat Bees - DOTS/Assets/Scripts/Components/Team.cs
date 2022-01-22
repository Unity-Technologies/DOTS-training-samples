using Unity.Entities;

public struct Team : IComponentData
{
    public TeamName Value;
}

public enum TeamName
{
    Undefined,
    A,
    B
}