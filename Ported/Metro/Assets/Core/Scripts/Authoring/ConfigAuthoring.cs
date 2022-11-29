using Unity.Entities;

class ConfigAuthoring : UnityEngine.MonoBehaviour
{
    public UnityEngine.GameObject PersonPrefab;
    public UnityEngine.GameObject PlatformPrefab;
    public UnityEngine.GameObject TrainPrefab;
    public UnityEngine.GameObject RailsPrefab;
    
    public int PersonCount;
    public int PlatformCountPerStation;
    public int NumberOfStations;
    public float SafeZoneRadius;
}

class ConfigBaker : Baker<ConfigAuthoring>
{
    public override void Bake(ConfigAuthoring authoring)
    {
        AddComponent(new Config
        {
            PersonPrefab = GetEntity(authoring.PersonPrefab),
            PlatformPrefab = GetEntity(authoring.PlatformPrefab),
            RailsPrefab = GetEntity(authoring.TrainPrefab),
            TrainPrefab = GetEntity(authoring.RailsPrefab),
            PersonCount = authoring.PersonCount,
            PlatformCountPerStation = authoring.PlatformCountPerStation,
            NumberOfStations = authoring.NumberOfStations,
            SafeZoneRadius = authoring.SafeZoneRadius
        });
    }
}