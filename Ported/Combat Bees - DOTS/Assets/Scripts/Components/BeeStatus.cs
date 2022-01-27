using Unity.Entities;

public struct BeeStatus : IComponentData
{
    public Status Value;
}

public enum Status
{
    Idle,
    Gathering,
    Attacking
}