using Unity.Entities;

public struct TrainConfig : IComponentData
{
    public Entity TrainPrefab;
    public Entity CarriagePrefab;
    public int CarriageCount;
}