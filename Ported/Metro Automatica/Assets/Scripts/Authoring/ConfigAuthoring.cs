using Unity.Entities;

class ConfigAuthoring : UnityEngine.MonoBehaviour
{
    public UnityEngine.GameObject StationPrefab;
    public int StationCount;
    public float SafeZoneRadius;
    public UnityEngine.GameObject RailPrefab;

    class ConfigBaker : Baker<ConfigAuthoring>
    {
        public override void Bake(ConfigAuthoring authoring)
        {
            AddComponent(new Config
            {
                StationPrefab = GetEntity(authoring.StationPrefab),
                RailPrefab = GetEntity(authoring.RailPrefab),
                StationCount = authoring.StationCount,
                SafeZoneRadius = authoring.SafeZoneRadius
            });
        }
    }
}    

struct Config : IComponentData
{
    public Entity StationPrefab;
    public Entity RailPrefab;
    public int StationCount;
    public float SafeZoneRadius;
}
