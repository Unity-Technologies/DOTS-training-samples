using Unity.Entities;

public struct BotChain : ISharedComponentData
{
    Entity startChain;
    Entity endChain;
    Entity headBot;
}