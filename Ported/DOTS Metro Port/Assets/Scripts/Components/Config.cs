using Unity.Entities;

struct Config : IComponentData
{
    public Entity RailPrefab;
    public Entity CarriagePrefab;
    public int TrainCount;
}