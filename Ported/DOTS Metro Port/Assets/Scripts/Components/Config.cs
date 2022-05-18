using Unity.Entities;

struct Config : IComponentData
{
    public Entity RailPrefab;
    public Entity TrainPrefab;
    public Entity CarriagePrefab;
    public Entity PlatformPrefab;
    public int TrainCount;
    public int CarriagesPerTrain;
    public float TrainOffset;
    public float CarriageLength;
    public float MaxTrainSpeed;
    public float TrainWaitTime;
}