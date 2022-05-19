using Unity.Entities;
using UnityEngine;

class ConfigAuthoring : UnityEngine.MonoBehaviour
{
    public UnityEngine.GameObject RailPrefab;
    public UnityEngine.GameObject TrainPrefab;
    public UnityEngine.GameObject CarriagePrefab;
    public UnityEngine.GameObject PlatformPrefab;
    public TrainSpawnType TrainSpawnType;
    public float TrainCount;
    public int CarriagesPerTrain = 4;
    public float TrainOffset;
    public float CarriageLength;
    public float MaxTrainSpeed;
    public float TrainWaitTime = 5;
}

class ConfigBaker : Baker<ConfigAuthoring>
{
    public override void Bake(ConfigAuthoring authoring)
    {
        AddComponent(new Config
        {
            RailPrefab = GetEntity(authoring.RailPrefab),
            CarriagePrefab = GetEntity(authoring.CarriagePrefab),
            TrainPrefab = GetEntity(authoring.TrainPrefab),
            PlatformPrefab = GetEntity(authoring.PlatformPrefab),
            TrainSpawnType = authoring.TrainSpawnType,
            TrainCount = authoring.TrainCount,
            CarriagesPerTrain = authoring.CarriagesPerTrain,
            TrainOffset = authoring.TrainOffset,
            CarriageLength = authoring.CarriageLength,
            MaxTrainSpeed = authoring.MaxTrainSpeed,
            TrainWaitTime = authoring.TrainWaitTime
        });
    }
}