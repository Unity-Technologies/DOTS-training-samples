using Unity.Entities;

public struct ParSpawnRuntime : IComponentData
{
    public Entity Prefab;
    public int SharkCount;
}
