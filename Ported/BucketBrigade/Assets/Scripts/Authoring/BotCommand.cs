using Unity.Entities;

public struct CurrentBotCommand : IComponentData
{
    public int Index;
    public Command Command;
}