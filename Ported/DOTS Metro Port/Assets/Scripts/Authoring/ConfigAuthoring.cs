using Unity.Entities;

class ConfigAuthoring : UnityEngine.MonoBehaviour
{
    public UnityEngine.GameObject RailPrefab;
    public UnityEngine.GameObject CarriagePrefab;
    public int TrainCount;
}

class ConfigBaker : Baker<ConfigAuthoring>
{
    public override void Bake(ConfigAuthoring authoring)
    {
        AddComponent(new Config
        {
            RailPrefab = GetEntity(authoring.RailPrefab),
            CarriagePrefab = GetEntity(authoring.CarriagePrefab),
            TrainCount = authoring.TrainCount
        });
    }
}