using Unity.Entities;

public enum WorkerStates
{
    Idle,
    Repositioning
}

public struct WorkerState : IComponentData
{
    public WorkerStates Value;
}

public struct Worker : IComponentData
{
    public Entity Team;    
}