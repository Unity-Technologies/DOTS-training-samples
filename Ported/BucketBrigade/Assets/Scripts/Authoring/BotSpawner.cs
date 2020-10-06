using Unity.Entities;

[GenerateAuthoringComponent]
public struct BotSpawner : IComponentData
{
    public Entity botPrefab;
    public int numberBots;
    public float spawnRadius;
}