using Unity.Entities;
using Unity.Mathematics;

public enum WorkerStates
{
    Idle,
    Repositioning
}

public struct WorkerState : IComponentData
{
    public WorkerStates Value;
}

public struct NextPosition : IComponentData, IEnableableComponent
{
    public float2 Value;
}