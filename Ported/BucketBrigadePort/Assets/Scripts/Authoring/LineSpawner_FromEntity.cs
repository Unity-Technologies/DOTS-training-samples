using Unity.Entities;

[GenerateAuthoringComponent]
public struct LineSpawner : IComponentData
{
    public int Count;
    public int CountOfFullPassBots;
    public int CountOfEmptyPassBots;
    public Entity LinePrefab;
    public Entity BotPrefab;
    public float BotSpeed;
}
