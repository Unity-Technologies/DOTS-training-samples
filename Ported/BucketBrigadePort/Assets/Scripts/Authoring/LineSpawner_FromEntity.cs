using Unity.Entities;

// ReSharper disable once InconsistentNaming
public struct LineSpawner_FromEntity : IComponentData
{
    public int Count;
    public int CountOfFullPassBots;
    public int CountOfEmptyPassBots;
    public Entity LinePrefab;
    public Entity BotPrefab;
}
