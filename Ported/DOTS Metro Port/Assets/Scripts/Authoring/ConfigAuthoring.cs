using Unity.Entities;

class ConfigAuthoring : UnityEngine.MonoBehaviour
{
    public UnityEngine.GameObject RailPrefab;
    public UnityEngine.GameObject TrainPrefab;
    public UnityEngine.GameObject CarriagePrefab;
    public int TrainCount;
    public int CarriagesPerTrain = 4;
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
            TrainCount = authoring.TrainCount,
            CarriagesPerTrain = authoring.CarriagesPerTrain
        });
    }
}