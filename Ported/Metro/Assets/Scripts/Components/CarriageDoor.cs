using Unity.Entities;

public enum CarriageDoorState
{
    Open,
    Opening,
    Close,
    Closing
}

public struct CarriageDoor : IComponentData
{
    public CarriageDoorState State;
}