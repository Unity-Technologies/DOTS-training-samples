using Unity.Entities;

[GenerateAuthoringComponent]
public struct GameConfig : IComponentData
{
    public Entity BeePrefab;
    public int BeeSpawnCount;
}