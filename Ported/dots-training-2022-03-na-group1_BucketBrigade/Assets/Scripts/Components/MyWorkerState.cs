using Unity.Entities;

public struct MyWorkerState : IComponentData
{
    public WorkerState Value;

    public MyWorkerState(WorkerState state)
    {
        Value = state;
    }
}

