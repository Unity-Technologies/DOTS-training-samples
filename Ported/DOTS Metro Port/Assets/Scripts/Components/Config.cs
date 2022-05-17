using Unity.Entities;

struct Config : IComponentData
{
    public Entity RailPrefab;
    public Entity TrainPrefab;
    public Entity CarriagePrefab;
    public int TrainCount;
    public int CarriagesPerTrain;
}