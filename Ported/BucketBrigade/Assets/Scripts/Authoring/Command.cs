using Unity.Entities;

public struct CommandBufferElement : IBufferElementData
{
    public Command Value;
}

public enum Command
{
    None,
    Move
}