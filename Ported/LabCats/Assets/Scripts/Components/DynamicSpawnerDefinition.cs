using Unity.Entities;

public struct DynamicSpawnerDefinition : IComponentData
{
    public Entity MousePrefab;
    public Entity CatPrefab;

    public int MaxCats;
    public float CatFrequency;
    public float MouseFrequency;

}
