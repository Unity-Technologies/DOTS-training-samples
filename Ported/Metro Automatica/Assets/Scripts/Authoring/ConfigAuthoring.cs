using Unity.Entities;

class ConfigAuthoring : UnityEngine.MonoBehaviour
{
    public UnityEngine.GameObject StationPrefab;
    public int StationCount;
    public float SafeZoneRadius;
    public UnityEngine.GameObject RailPrefab;
    public UnityEngine.GameObject WagonPrefab;
    public int WagonCount;

    class ConfigBaker : Baker<ConfigAuthoring>
    {
        public override void Bake(ConfigAuthoring authoring)
        {
            AddComponent(new Config
            {
                StationPrefab = GetEntity(authoring.StationPrefab),
                RailPrefab = GetEntity(authoring.RailPrefab),
                WagonPrefab = GetEntity(authoring.WagonPrefab),
                StationCount = authoring.StationCount,
                WagonCount = authoring.WagonCount,
                SafeZoneRadius = authoring.SafeZoneRadius
            });
        }
    }
}    

struct Config : IComponentData
{
    public Entity StationPrefab;
    public Entity RailPrefab;
    public Entity WagonPrefab;
    public int WagonCount;
    public int StationCount;
    public float SafeZoneRadius;
}
