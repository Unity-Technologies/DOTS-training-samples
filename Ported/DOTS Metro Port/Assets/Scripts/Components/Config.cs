using Unity.Entities;

public enum TrainSpawnType
{
    Absolute,
    Per100Metres,
}

public struct Config : IComponentData
{
    public Entity RailPrefab;
    public Entity TrainPrefab;
    public Entity CarriagePrefab;
    public Entity PlatformPrefab;
    public TrainSpawnType TrainSpawnType;
    public float TrainCount;
    public int CarriagesPerTrain;
    public float TrainOffset;
    public float CarriageLength;
    public float MaxTrainSpeed;
    public float TrainWaitTime;
}