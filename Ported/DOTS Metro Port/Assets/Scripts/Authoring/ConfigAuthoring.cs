using Unity.Entities;

class ConfigAuthoring : UnityEngine.MonoBehaviour
{
    public UnityEngine.GameObject RailPrefab;
    public UnityEngine.GameObject TrainPrefab;
    public UnityEngine.GameObject CarriagePrefab;
    public UnityEngine.GameObject PlatformPrefab;
    public int TrainCount;
    public int CarriagesPerTrain = 4;
    public float TrainOffset;
    public float CarriageLength;
    public float MaxTrainSpeed;
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
            TrainCount = authoring.TrainCount,
            CarriagesPerTrain = authoring.CarriagesPerTrain,
            TrainOffset = authoring.TrainOffset,
            CarriageLength = authoring.CarriageLength,
            MaxTrainSpeed = authoring.MaxTrainSpeed,
        }) ;
    }
}